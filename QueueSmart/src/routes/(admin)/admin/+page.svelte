<script lang="ts">
    import { services } from '$lib/stores/adminStore';

    function toggleQueue(id: number) {
        services.update(list =>
            (list || []).map(s => s.id === id ? { ...s, isOpen: !s.isOpen } : s)
        );
    }

    const priorityColor: Record<string, string> = {
        high:   'text-red-500 bg-red-500/10 border-red-500/20',
        medium: 'text-yellow-400 bg-yellow-500/10 border-yellow-500/20',
        low:    'text-teal bg-teal/10 border-teal/20',
    };

    $: svc = $services ?? [];

    $: stats = [
        { label: 'Total Services', value: svc.length },
        { label: 'Open services', value: svc.filter(s => s.isOpen).length },
        { label: 'People in Queue', value: svc.reduce((a, s) => a + (s.queueLength ?? 0), 0) },
    ];
</script>

<div class="p-8 space-y-8">
    <!-- Header -->
    <div>
        <h2 class="text-2xl font-bold">Dashboard</h2>
        <p class="text-navy-600 text-sm mt-1">Live overview of all queues and services.</p>
    </div>

    <!-- Stats row -->
    <div class="grid grid-cols-3 gap-4">
        {#each stats as stat}
            <div class="bg-navy-800 border border-navy-600 rounded-xl p-5">
                <p class="text-navy-600 text-xs uppercase tracking widest">{stat.label}</p>
                <p class="text-4xl font-bold text-teal mt-2">{stat.value}</p>
            </div>
        {/each}
    </div>

    <!-- Services table -->
    <div class="bg-navy-800 border border-navy-600 rounded-xl overflow-hidden">
        <div class="px-6 py-4 border-b border-navy-600 flex justify-between items-center">
            <h3 class="font-semibold text-white">Services</h3>
            <a href="/admin/services"
                class="text-xs text-teal hover:text-teal/80 transition-colors">
                Manage →
            </a>
        </div>

        <table class="w-full text-sm">
            <thead class="bg-navy-700/50">
                <tr class="text-left text-navy-600 text-xs uppercase tracking-widest">
                    <th class="px-6 py-3">Service</th>
                    <th class="px-6 py-3">Priority</th>
                    <th class="px-6 py-3">Queue</th>
                    <th class="px-6 py-3">Status</th>
                    <th class="px-6 py-3">Actions</th>
                </tr>
            </thead>
            <tbody class="divide-y divide-navy-600">
                {#each svc as service (service.id)}
                    <tr class="hover:bg-navy-700/30 transition-colors">
                        <td class="px-6 py-4 text-white font-medium">{service.name}</td>
                        <td class="px-6 py-4">
                            <span class={
                                `text-xs px-2 py-0.5 rounded border ${priorityColor[(service.priority || '').toLowerCase()] ?? ''}`
                            }>
                                {service.priority}
                            </span>
                        </td>
                        <td class="px-6 py-4 text-navy-400">{service.queueLength} waiting</td>
                        <td class="px-6 py-4">
                            <span class={
                                `text-xs px-2 py-0.5 rounded border ${service.isOpen
                                    ? 'text-teal bg-teal/10 border-teal/20'
                                    : 'text-navy-600 bg-navy-700 border-navy-600'}`
                                }>{service.isOpen ? 'Open' : 'Closed'}</span>
                        </td>
                        <td class="px-6 py-4">
                            <button
                                type="button"
                                class={
                                    `text-xs px-3 py-1.5 rounded border transition-all ${service.isOpen
                                        ? 'border-red-500/30 text-red-400 hover:bg-red-500/10'
                                        : 'border-teal/30 text-teal hover:bg-teal/10'}`
                                }>
                                {service.isOpen ? 'Close Queue' : 'Open Queue'}
                            </button>
                        </td>
                    </tr>
                {/each}
            </tbody>
        </table>
    </div>
</div>