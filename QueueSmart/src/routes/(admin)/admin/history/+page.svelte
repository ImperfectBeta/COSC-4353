<script lang="ts">
	import { onMount } from "svelte";
	import { fetchHistory, fetchStatistics } from "$lib/api/services";
	import type { HistoryEntry, StatisticsResponse } from "$lib/api/services";

	let history: HistoryEntry[] = [];
	let stats: StatisticsResponse | null = null;
	let loading = true;
	let error = "";
	let filterServiceId = "";

	onMount(async () => {
		await loadData();
	});

	async function loadData() {
		loading = true;
		error = "";
		try {
			const [historyData, statsData] = await Promise.all([
				fetchHistory(filterServiceId || undefined),
				fetchStatistics(),
			]);
			history = historyData;
			stats = statsData;
		} catch (err: any) {
			error = err.message || "Failed to load history data.";
		} finally {
			loading = false;
		}
	}

	function formatTimestamp(ts: string): string {
		const date = new Date(ts);
		return date.toLocaleString();
	}

	const actionColor: Record<string, string> = {
		Created: "text-primary bg-primary/10 border border-primary/20",
		Updated: "text-yellow-400 bg-yellow-500/10 border border-yellow-500/20",
		Deleted:
			"text-destructive bg-destructive/10 border border-destructive/20",
	};
</script>

<div class="p-8 space-y-8">
	<div>
		<h2 class="text-2xl font-bold text-background">Service History</h2>
		<p class="text-background text-sm mt-1">
			Track changes and activity across all services.
		</p>
	</div>

	{#if error}
		<div
			class="bg-destructive/10 border border-destructive/20 text-destructive rounded-lg p-4 text-sm"
		>
			{error}
		</div>
	{/if}

	{#if loading}
		<p class="text-accent text-sm">Loading history...</p>
	{:else}
		<!-- Stats Row -->
		{#if stats}
			<div class="grid grid-cols-3 gap-4">
				<div
					class="bg-primary/10 border border-primary/20 rounded-xl p-5"
				>
					<p
						class="text-background text-xs uppercase tracking-widest"
					>
						Total Services
					</p>
					<p class="text-4xl font-bold text-primary mt-2">
						{stats.totalServices}
					</p>
				</div>
				<div
					class="bg-primary/10 border border-primary/20 rounded-xl p-5"
				>
					<p
						class="text-background text-xs uppercase tracking-widest"
					>
						Active Services
					</p>
					<p class="text-4xl font-bold text-primary mt-2">
						{stats.activeServices}
					</p>
				</div>
				<div
					class="bg-primary/10 border border-primary/20 rounded-xl p-5"
				>
					<p
						class="text-background text-xs uppercase tracking-widest"
					>
						History Entries
					</p>
					<p class="text-4xl font-bold text-primary mt-2">
						{stats.totalHistoryEntries}
					</p>
				</div>
			</div>
		{/if}

		<!-- History Table -->
		<div
			class="bg-primary/10 border border-primary/20 rounded-xl overflow-hidden"
		>
			<div class="px-6 py-4 border-b border-primary/20">
				<h3 class="font-semibold text-primary-foreground">
					Activity Log
				</h3>
			</div>

			{#if history.length === 0}
				<div class="px-6 py-8 text-center">
					<p class="text-accent text-sm">
						No history entries yet. Create, edit, or delete a
						service to see activity here.
					</p>
				</div>
			{:else}
				<table class="w-full text-sm">
					<thead class="bg-primary/10">
						<tr
							class="text-left text-background text-xs uppercase tracking-widest"
						>
							<th class="px-6 py-3">Timestamp</th>
							<th class="px-6 py-3">Service</th>
							<th class="px-6 py-3">Action</th>
							<th class="px-6 py-3">Details</th>
						</tr>
						<tr>
							<td colspan="4" class="h-px bg-primary/25"></td>
						</tr>
					</thead>
					<tbody class="divide-y divide-primary/25">
						{#each history as entry (entry.id)}
							<tr class="hover:bg-primary/10 transition-colors">
								<td
									class="px-6 py-4 text-background whitespace-nowrap"
									>{formatTimestamp(entry.timestamp)}</td
								>
								<td
									class="px-6 py-4 text-primary-foreground font-medium"
									>{entry.serviceName}</td
								>
								<td class="px-6 py-4">
									<span
										class={`text-xs px-2 py-0.5 rounded ${actionColor[entry.action] ?? ""}`}
									>
										{entry.action}
									</span>
								</td>
								<td class="px-6 py-4 text-background"
									>{entry.details || "—"}</td
								>
							</tr>
						{/each}
					</tbody>
				</table>
			{/if}
		</div>
	{/if}
</div>
