<script lang="ts">
	import {
		XIcon,
		ClockIcon,
		Building2Icon,
		TicketIcon,
	} from "@lucide/svelte";
	import Button from "./button.svelte";
	import type { Queue, QueueEntry } from "$lib/types";
	import { fade, scale } from "svelte/transition";
	import { quintOut } from "svelte/easing";

	let {
		open = false,
		queue,
		entry,
		onclose,
	}: {
		open: boolean;
		queue?: Queue;
		entry?: QueueEntry;
		onclose: () => void;
	} = $props();
</script>

{#if open && queue && entry}
	<div
		class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6"
		role="dialog"
		aria-modal="true"
	>
		<div
			class="absolute inset-0 bg-muted/80 backdrop-blur-sm transition-all duration-200"
			in:fade={{ duration: 300 }}
			out:fade={{ duration: 200 }}
			onclick={onclose}
			onkeydown={e => {
				if (e.key === "Escape") onclose();
			}}
			role="none"
		></div>

		<div
			class="relative w-full max-w-md overflow-hidden rounded-3xl border border-accent/20 bg-background shadow-2xl shadow-accent/20"
			in:scale={{
				duration: 300,
				easing: quintOut,
				start: 0.95,
				opacity: 0,
			}}
			out:scale={{ duration: 200, start: 0.95, opacity: 0 }}
		>
			<div
				class="h-40 bg-accent/5 relative overflow-hidden border-b border-accent/10"
			>
				<div
					class="absolute -top-10 -right-10 w-48 h-48 bg-primary/20 rounded-full blur-[60px] animate-pulse-slow"
				></div>
				<div
					class="absolute -bottom-10 -left-10 w-48 h-48 bg-accent/20 rounded-full blur-[60px] animate-pulse-slower"
				></div>

				<div class="absolute top-4 right-4 z-10">
					<button
						onclick={onclose}
						class="rounded-full p-2 text-muted/60 cursor-pointer hover:bg-accent/10 hover:text-foreground"
						aria-label="Close modal"
					>
						<XIcon size={20} />
					</button>
				</div>

				<div
					class="absolute bottom-6 left-6 right-6 flex items-end justify-between"
				>
					<div class="flex items-center gap-4">
						<div
							class="p-3 bg-background/90 backdrop-blur-md rounded-2xl shadow-sm ring-1 ring-accent/10"
						>
							<TicketIcon class="size-6 text-primary" />
						</div>
						<div>
							<p
								class="text-xs font-bold uppercase tracking-wider text-accent/80 mb-0.5"
							>
								Your Ticket
							</p>
							<h2
								class="font-arvo font-bold text-xl leading-none text-foreground tracking-tight"
							>
								{queue.name}
							</h2>
						</div>
					</div>
				</div>
			</div>

			<div class="p-6 sm:p-8 space-y-8">
				<div class="flex flex-col items-center justify-center">
					<p
						class="text-xs font-bold uppercase tracking-widest text-muted/60 mb-6"
					>
						Current Position
					</p>

					<div class="relative group cursor-default">
						<div
							class="absolute inset-0 rounded-full border border-accent/10 scale-[1.3]"
						></div>
						<div
							class="absolute inset-0 rounded-full border border-dashed border-primary/20 animate-spin-slow scale-[1.15]"
						></div>

						<div
							class="relative size-36 rounded-full bg-background flex flex-col items-center justify-center border-4 border-accent/5 shadow-xl ring-4 ring-background z-10 transition-transform duration-500 group-hover:scale-105"
						>
							<span
								class="font-arvo text-6xl font-black text-foreground tabular-nums tracking-tighter"
							>
								{entry.position}
							</span>
							<span
								class="text-[10px] font-bold text-muted/50 uppercase tracking-widest mt-1"
							>
								In Line
							</span>
						</div>

						<div
							class="absolute inset-0 bg-primary/20 blur-3xl -z-10 opacity-50 group-hover:opacity-70 transition-opacity duration-500"
						></div>
					</div>
				</div>

				<div class="grid grid-cols-2 gap-4">
					<div
						class="group bg-accent/5 hover:bg-accent/10 rounded-2xl p-4 flex flex-col items-center justify-center text-center gap-1 border border-transparent hover:border-accent/10"
					>
						<ClockIcon
							class="size-5 text-primary mb-2 transition-transform duration-300 group-hover:scale-110"
						/>
						<span
							class="text-xs text-muted/70 font-bold uppercase tracking-wide"
							>Est. Wait</span
						>
						<span
							class="font-arvo font-bold text-xl text-foreground"
						>
							{entry.estimatedWaitTime}<span
								class="text-sm font-normal text-muted/60 ml-1"
								>min</span
							>
						</span>
					</div>

					<div
						class="group bg-accent/5 hover:bg-accent/10 rounded-2xl p-4 flex flex-col items-center justify-center text-center gap-1 border border-transparent hover:border-accent/10"
					>
						<Building2Icon
							class="size-5 text-accent mb-2 transition-transform duration-300 group-hover:scale-110"
						/>
						<span
							class="text-xs text-muted/70 font-bold uppercase tracking-wide"
							>Location</span
						>
						<span
							class="font-arvo font-bold text-sm text-foreground line-clamp-1 w-full"
						>
							{queue.organization}
						</span>
					</div>
				</div>

				<div
					class="bg-muted text-muted-foreground rounded-xl p-4 text-xs/relaxed text-center font-medium"
				>
					<p>
						Please arrive 5 minutes before your estimated time. You
						will receive a notification when it's your turn.
					</p>
				</div>

				<div class="pt-2">
					<Button
						variant="primary"
						class="w-full justify-center h-12 text-base shadow-lg shadow-primary/20 hover:shadow-primary/30 active:scale-[0.98]"
						onclick={onclose}
					>
						Close Ticket
					</Button>
				</div>
			</div>
		</div>
	</div>
{/if}

<style>
	@keyframes spin-slow {
		from {
			transform: rotate(0deg) scale(1.15);
		}
		to {
			transform: rotate(360deg) scale(1.15);
		}
	}

	@keyframes pulse-slow {
		0%,
		100% {
			opacity: 0.5;
			transform: scale(1);
		}
		50% {
			opacity: 0.8;
			transform: scale(1.1);
		}
	}

	@keyframes pulse-slower {
		0%,
		100% {
			opacity: 0.3;
			transform: scale(1);
		}
		50% {
			opacity: 0.6;
			transform: scale(1.2);
		}
	}

	.animate-spin-slow {
		animation: spin-slow 20s linear infinite;
	}

	.animate-pulse-slow {
		animation: pulse-slow 4s ease-in-out infinite;
	}

	.animate-pulse-slower {
		animation: pulse-slower 6s ease-in-out infinite;
	}
</style>
