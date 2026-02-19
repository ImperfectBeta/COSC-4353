<script lang="ts">
	import type { PageData } from "./$types";
	import QueueCard from "$lib/components/queue-card.svelte";
	import Search from "@lucide/svelte/icons/search";

	let { data }: { data: PageData } = $props();

	let searchQuery = $state("");

	let filteredQueues = $derived(
		data.suggestedQueues.filter(
			q =>
				q.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
				q.organization
					.toLowerCase()
					.includes(searchQuery.toLowerCase()),
		),
	);
</script>

<div class="space-y-8">
	<section>
		<h1 class="text-3xl font-bold tracking-tight">
			Welcome back, {data.user.name}
		</h1>
		<p class="text-foreground/70 mt-2">
			Here is what's happening with your queues today.
		</p>
	</section>

	<section class="space-y-4">
		<h2 class="text-xl font-semibold tracking-tight">Active Queues</h2>
		{#if data.activeQueues.length === 0}
			<div
				class="rounded-lg border border-dashed p-8 text-center text-muted-foreground"
			>
				You are not currently in any queues.
			</div>
		{:else}
			<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
				{#each data.activeQueues as entry}
					<QueueCard queue={entry.queue} {entry} section="active" />
				{/each}
			</div>
		{/if}
	</section>

	<section class="space-y-4">
		<div
			class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between"
		>
			<h2 class="text-xl font-semibold tracking-tight">
				Discover Queues
			</h2>

			<div class="relative w-full max-w-sm group">
				<div
					class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3"
				>
					<Search
						class="h-4 w-4 text-muted/70 group-hover:text-muted group-focus-within:text-muted"
					/>
				</div>
				<input
					type="search"
					placeholder="Find a queue"
					bind:value={searchQuery}
					class="w-full px-3 py-1 pl-9"
				/>
			</div>
		</div>

		<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
			{#each filteredQueues as queue}
				<QueueCard {queue} />
			{/each}
		</div>
	</section>
</div>
