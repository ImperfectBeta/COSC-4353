<script lang="ts">
    import { onMount } from "svelte";
    import { get } from "svelte/store";
    import { services, loadServices } from "$lib/stores/adminStore";
    import { authSession } from "$lib/stores/auth";

    async function toggleQueue(id: string) {
        const currentServices = get(services) || [];
        const targetService = currentServices.find(s => s.id === id);
        
        if (!targetService) return;
        
        const isCurrentlyOpen = targetService.isOpen;
        const action = isCurrentlyOpen ? "close" : "open";
        const httpMethod = isCurrentlyOpen ? "PUT" : "POST";

        try {
            const session = get(authSession);
            const res = await fetch(`/api/queues/${action}/${id}`, {
                method: httpMethod,
                headers: {
                    "Content-Type": "application/json",
                    ...(session?.token ? { "Authorization": `Bearer ${session.token}` } : {})
                }
            });

            if (!res.ok) {
                const errorData = await res.json().catch(() => null);
                throw new Error(errorData?.message || `Failed to ${action} queue.`);
            }

            services.update(list =>
                (list || []).map(s =>
                    s.id === id ? { ...s, isOpen: !isCurrentlyOpen } : s,
                ),
            );
        } catch (error: any) {
            console.error("Toggle queue error:", error);
            alert(`Could not ${action} queue: ${error.message}`);
        }
    }

    const priorityColor: Record<string, string> = {
        high: "text-red-400 bg-red-500/20 border-red-500/30",
        medium: "text-yellow-400 bg-yellow-500/20 border-yellow-500/30",
        low: "text-[#5BC0BE] bg-[#5BC0BE]/20 border-[#5BC0BE]/30",
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

    <!-- Header -->
<div class="p-8 md:p-10 space-y-8 max-w-7xl mx-auto font-['Inter']">
    <div>
        <h2 class="text-3xl font-bold text-white">Dashboard</h2>
        <p class="text-white/60 text-sm mt-2">
            Live overview of all queues and services.
        </p>
    </div>

    <!-- Stats row -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        {#each stats as stat}
            <div class="bg-[#1C2541] border border-white/10 rounded-[16px] p-6 shadow-lg">
                <p class="text-white/60 text-xs uppercase tracking-widest font-semibold">
                    {stat.label}
                </p>
                <p class="text-5xl font-bold text-[#5BC0BE] mt-3">{stat.value}</p>
            </div>
        {/each}
    </div>

    <!-- Services table -->
    <div class="bg-[#1C2541] border border-white/10 rounded-[16px] overflow-hidden shadow-lg">
        <div class="px-6 py-5 border-b border-white/10 flex justify-between items-center">
            <h3 class="font-bold text-lg text-white">Services</h3>
            <a
                href="/admin/services"
                class="text-sm text-[#5BC0BE] hover:text-[#4aa8a6] font-medium transition-colors"
            >
                Manage &rarr;
            </a>
        </div>

        <div class="overflow-x-auto">
            <table class="w-full text-sm">
                <thead class="bg-[#0B132B]/50">
                    <tr class="text-left text-white/60 text-xs uppercase tracking-widest">
                        <th class="px-6 py-4 font-semibold">Service</th>
                        <th class="px-6 py-4 font-semibold">Priority</th>
                        <th class="px-6 py-4 font-semibold">Queue</th>
                        <th class="px-6 py-4 font-semibold">Status</th>
                        <th class="px-6 py-4 font-semibold">Actions</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-white/10">
                    {#each svc as service (service.id)}
                        <tr class="hover:bg-white/5 transition-colors">
                            <td class="px-6 py-4 text-white font-medium">{service.name}</td>
                            <td class="px-6 py-4">
                                <span class={`text-xs px-2.5 py-1 rounded-[6px] border font-medium ${priorityColor[(service.priority || "").toLowerCase()] ?? ""}`}>
                                    {service.priority}
                                </span>
                            </td>
                            <td class="px-6 py-4 text-white/80">{service.queueLength} waiting</td>
                            <td class="px-6 py-4">
                                <span class={`text-xs px-2.5 py-1 rounded-[6px] border font-medium ${
                                    !service.isOpen
                                        ? "text-gray-400 bg-gray-500/20 border-gray-500/30"
                                        : "text-[#5BC0BE] bg-[#5BC0BE]/10 border-[#5BC0BE]/30"
                                }`}>
                                    {service.isOpen ? "Open" : "Closed"}
                                </span>
                            </td>
                            <td class="px-6 py-4">
                                <button
                                    type="button"
                                    on:click={() => toggleQueue(service.id)}
                                    class={`text-xs px-3 py-1.5 rounded-[6px] border font-medium cursor-pointer transition-all ${
                                        service.isOpen
                                            ? "border-red-500/30 text-red-400 hover:bg-red-500/20"
                                            : "border-[#5BC0BE]/30 text-[#5BC0BE] hover:bg-[#5BC0BE]/20"
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
</div>