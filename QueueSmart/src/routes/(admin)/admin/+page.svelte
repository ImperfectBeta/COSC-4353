<script lang="ts">
	import { onMount } from "svelte";
	import { services, loadServices } from "$lib/stores/adminStore";

	function toggleQueue(id: string) {
		services.update(list =>
			(list || []).map(s =>
				s.id === id ? { ...s, isOpen: !s.isOpen } : s,
			),
		);
	}

	const priorityColor: Record<string, string> = {
		high: "text-red-500 bg-red-500/10 border-red-500/20",
		medium: "text-yellow-400 bg-yellow-500/10 border-yellow-500/20",
		low: "text-green-400 bg-green-500/10 border-green-500/20",
	};

	$: svc = $services ?? [];

	$: stats = [
		{ label: "Total Services", value: svc.length },
		{ label: "Open services", value: svc.filter(s => s.isOpen).length },
		{
			label: "People in Queue",
			value: svc.reduce((a, s) => a + (s.queueLength ?? 0), 0),
		},
	];

	onMount(() => {
		loadServices();
	});
</script>

<div class="p-8 space-y-8">
	<!-- Header -->
	<div>
		<h2 class="text-2xl font-bold">Dashboard</h2>
		<p class="text-navy-600 text-sm mt-1">
			Live overview of all queues and services.
		</p>
	</div>

	<!-- Stats row -->
	<div class="grid grid-cols-3 gap-4">
		{#each stats as stat}
			<div class="bg-primary/10 border border-primary/20 rounded-xl p-5">
				<p class="text-navy-600 text-xs uppercase tracking widest">
					{stat.label}
				</p>
				<p class="text-4xl font-bold text-teal mt-2">{stat.value}</p>
			</div>
		{/each}
	</div>

	<!-- Services table -->
	<div
		class="bg-primary/10 border border-primary/20 rounded-xl overflow-hidden"
	>
		<div
			class="px-6 py-4 border-b border-primary/20 flex justify-between items-center"
		>
			<h3 class="font-semibold text-white">Services</h3>
			<a
				href="/admin/services"
				class="text-xs text-teal hover:text-teal/80 transition-colors"
			>
				Manage →
			</a>
		</div>

		<table class="w-full text-sm">
			<thead class="bg-primary/10">
				<tr
					class="text-left text-navy-600 text-xs uppercase tracking-widest"
				>
					<th class="px-6 py-3">Service</th>
					<th class="px-6 py-3">Priority</th>
					<th class="px-6 py-3">Queue</th>
					<th class="px-6 py-3">Status</th>
					<th class="px-6 py-3">Actions</th>
				</tr>
				<tr>
					<td colspan="5" class="h-px bg-primary/25"></td>
				</tr>
			</thead>
			<tbody class="divide-y divide-primary/25">
				{#each svc as service (service.id)}
					<tr class="hover:bg-primary/10 transition-colors">
						<td class="px-6 py-4 text-white font-medium"
							>{service.name}</td
						>
						<td class="px-6 py-4">
							<span
								class={`text-xs px-2 py-0.5 rounded border ${priorityColor[(service.priority || "").toLowerCase()] ?? ""}`}
							>
								{service.priority}
							</span>
						</td>
						<td class="px-6 py-4 text-background"
							>{service.queueLength} waiting</td
						>
						<td class="px-6 py-4">
							<span
								class={`text-xs px-2 py-0.5 rounded border ${
									!service.isOpen
										? "text-accent bg-accent/25 border-accent/50"
										: "text-primary bg-primary/10 border-primary/20"
								}`}>{service.isOpen ? "Open" : "Closed"}</span
							>
						</td>
						<td class="px-6 py-4">
							<button
								type="button"
								on:click={() => toggleQueue(service.id)}
								class={`text-xs px-3 py-1.5 rounded border cursor-pointer transition-all ${
									service.isOpen
										? "border-red-500/30 text-red-400 hover:bg-red-500/10"
										: "border-primary/30 text-primary hover:bg-primary/10"
								}`}
							>
								{service.isOpen ? "Close Queue" : "Open Queue"}
							</button>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>
