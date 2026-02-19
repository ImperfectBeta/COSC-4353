<script lang="ts">
	import Button from "./button.svelte";
	import Badge, { type BadgeVariant } from "./badge.svelte";
	import type { Queue, QueueEntry, QueueStatus } from "$lib/types";
	import clsx from "clsx";
	import type { Component } from "svelte";
	import {
		Building2Icon,
		CircleCheckBigIcon,
		CirclePlusIcon,
		InfoIcon,
		PauseIcon,
		PlusIcon,
		TicketIcon,
		XIcon,
	} from "@lucide/svelte";

	let {
		queue,
		entry,
		section = "discoverable",
		onviewticket,
	}: {
		queue: Queue;
		entry?: QueueEntry; // only present if user is in this queue
		section?: "active" | "discoverable";
		onviewticket?: (entryId: string) => void;
	} = $props();

	function getQueueStatusBadgeUI(status: QueueStatus): {
		variant: BadgeVariant;
		icon: Component;
	} {
		switch (status) {
			case "open":
				return { variant: "success", icon: CircleCheckBigIcon };
			case "closed":
				return { variant: "destructive", icon: XIcon };
			case "paused":
				return { variant: "warning", icon: PauseIcon };
			default:
				return { variant: "secondary", icon: InfoIcon };
		}
	}

	let { variant, icon } = $derived(getQueueStatusBadgeUI(queue.status));
</script>

<div
	class={clsx(
		"rounded-xl font-nunito bg-muted bg-card text-muted-foreground shadow-md shadow-accent/50 p-6 flex flex-col gap-4",
	)}
>
	<div class="flex justify-between items-start">
		<div>
			<h3 class="font-semibold leading-none text-lg tracking-tight">
				{queue.name}
			</h3>
			<p class="text-sm font-medium mt-1.5">
				<Building2Icon class="mr-0.5 mb-0.5 size-3.5 inline" />
				{queue.organization}
			</p>
		</div>
		<Badge {variant} {icon}>
			{queue.status}
		</Badge>
	</div>

	<p class="text-sm opacity-70 line-clamp-2">
		{queue.description}
	</p>

	<div class="flex items-center gap-2 text-sm mt-auto">
		<div class="flex flex-col bg-accent p-1.5 rounded-sm">
			<span class="opacity-85 text-xs">Est. Wait Time</span>
			<span class="font-semibold">~{queue.averageWaitTime} min</span>
		</div>
		<div class="flex flex-col bg-accent p-1.5 rounded-sm">
			<span class="opacity-85 text-xs">Queue Length</span>
			<span class="font-semibold">{queue.currentLength} people</span>
		</div>
		{#if entry}
			<div class="flex flex-col bg-accent p-1.5 rounded-sm">
				<span class="opacity-85 text-xs">Your Position</span>
				<span class="font-semibold text-primary">#{entry.position}</span
				>
			</div>
		{/if}
	</div>

	<div class="mt-4 pt-4 border-t border-muted-foreground/15 flex justify-end">
		{#if section === "active" && entry}
			<div class="flex items-center gap-2">
				<Button
					variant="destructive"
					size="sm"
					onclick={() => console.log("Leave Queue", entry.id)}
				>
					<XIcon size={18} />
					Leave
				</Button>
				<Button
					variant="primary"
					size="sm"
					class="bg-primary/80 hover:bg-primary"
					onclick={() => onviewticket?.(entry.id)}
				>
					<TicketIcon size={18} />
					View Ticket
				</Button>
			</div>
		{:else}
			<Button
				variant={queue.status === "open" ? "outline" : "ghost"}
				size="sm"
				onclick={() => console.log("Join Queue", queue.id)}
				disabled={queue.status !== "open"}
				class={queue.status === "open"
					? "gap-1"
					: "text-muted-foreground"}
			>
				{#if queue.status === "open"}
					<PlusIcon size={18} />
				{/if}
				{queue.status === "open" ? "Join Queue" : "Unavailable"}
			</Button>
		{/if}
	</div>
</div>
