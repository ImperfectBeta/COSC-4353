<script lang="ts">
    import { onMount } from "svelte";
    import { fetchReportStatistics, downloadPdfReport, type ReportStatistics, type ReportFilters } from "$lib/api/reports";
    import { services, loadServices } from "$lib/stores/adminStore";

    let stats: ReportStatistics | null = null;
    let loading = true;
    let downloading = false;
    let initialLoadComplete = false;
    let startDate = "";
    let endDate = "";
    let selectedServiceIds: string[] = [];
    let selectedStatus = "";
    let minWait: number | null = null;
    let maxWait: number | null = null;

    $: if (minWait !== null && minWait < 0) minWait = 0;
    $: if (maxWait !== null && maxWait < 0) maxWait = 0;
    $: if (minWait !== null && maxWait !== null && maxWait < minWait) {
        maxWait = minWait;
    }

    $: if ($services && $services.length > 0 && !initialLoadComplete) {
        selectedServiceIds = $services.map(s => s.id);
        initialLoadComplete = true;
        loadData();
    }

    function toggleService(id: string) {
        if (selectedServiceIds.includes(id)) {
            selectedServiceIds = selectedServiceIds.filter(s => s !== id);
        } else {
            selectedServiceIds = [...selectedServiceIds, id];
        }
    }

    async function loadData() {
        loading = true;
        try {
            const filters: ReportFilters = {
                startDate: startDate || undefined,
                endDate: endDate || undefined,
                serviceIds: selectedServiceIds,
                status: selectedStatus || undefined,
                minWait: minWait ?? undefined,
                maxWait: maxWait ?? undefined
            };
            stats = await fetchReportStatistics(filters);
        } catch (err) {
            console.error(err);
        } finally {
            loading = false;
        }
    }

    async function handleDownload() {
        downloading = true;
        try {
            const filters: ReportFilters = {
                startDate: startDate || undefined,
                endDate: endDate || undefined,
                serviceIds: selectedServiceIds,
                status: selectedStatus || undefined,
                minWait: minWait ?? undefined,
                maxWait: maxWait ?? undefined
            };
            await downloadPdfReport(filters);
        } catch (err) {
            alert("Failed to download the PDF report.");
        } finally {
            downloading = false;
        }
    }

    onMount(() => {
        loadServices();
    });
</script>

<div class="p-8 space-y-8">
    <div>
        <h2 class="text-2xl font-bold text-background">Report Module</h2>
        <p class="text-sm mt-1 text-background">
            Generate and export queue participation and usage statistics.
        </p>
    </div>

    <div class="p-6 rounded-xl space-y-6" style="background:var(--color-muted); border:1px solid rgba(0,0,0,0.12)">
        <div class="flex items-center justify-between">
            <h3 class="font-semibold text-white">Report Filters</h3>
            <button on:click={loadData} class="px-6 py-2 bg-[#5BC0BE] text-[#1C2541] font-bold text-sm rounded shadow hover:bg-teal-400 transition-colors">
                Apply Filters
            </button>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div class="space-y-3">
                <span class="text-xs font-bold text-navy-600 uppercase tracking-wider block mb-1">Date Range</span>
                <div class="flex flex-col gap-2">
                    <input type="date" aria-label="Start Date" bind:value={startDate} class="px-3 py-2 rounded bg-zinc-900 border border-zinc-700 text-sm text-white focus:outline-none focus:border-teal-500 w-full">
                    <input type="date" aria-label="End Date" bind:value={endDate} class="px-3 py-2 rounded bg-zinc-900 border border-zinc-700 text-sm text-white focus:outline-none focus:border-teal-500 w-full">
                </div>
            </div>

            <div class="space-y-3">
                <span class="text-xs font-bold text-navy-600 uppercase tracking-wider block mb-1">Services</span>
                <div class="flex flex-col gap-2 max-h-[100px] overflow-y-auto pr-2">
                    {#each $services as service}
                        <label for="service-{service.id}" class="flex items-center gap-2 text-sm text-white/80 cursor-pointer hover:text-white">
                            <input 
                                id="service-{service.id}"
                                type="checkbox" 
                                checked={selectedServiceIds.includes(service.id)}
                                on:change={() => toggleService(service.id)}
                                class="rounded border-zinc-700 text-[#5BC0BE] focus:ring-[#5BC0BE] bg-zinc-900"
                            >
                            {service.name}
                        </label>
                    {/each}
                    {#if $services.length === 0}
                        <span class="text-xs text-zinc-500">No services available.</span>
                    {/if}
                </div>
            </div>

            <div class="space-y-3">
                <span class="text-xs font-bold text-navy-600 uppercase tracking-wider block mb-1">Wait Time (Min)</span>
                <div class="flex items-center gap-2">
                    <input type="number" min="0" aria-label="Minimum Wait Time" placeholder="Min" bind:value={minWait} class="px-3 py-2 rounded bg-zinc-900 border border-zinc-700 text-sm text-white focus:outline-none focus:border-teal-500 w-full">
                    <span class="text-zinc-500">-</span>
                    <input type="number" min="0" aria-label="Maximum Wait Time" placeholder="Max" bind:value={maxWait} class="px-3 py-2 rounded bg-zinc-900 border border-zinc-700 text-sm text-white focus:outline-none focus:border-teal-500 w-full">
                </div>
            </div>

            <div class="space-y-3">
                <label for="statusFilter" class="text-xs font-bold text-navy-600 uppercase tracking-wider block mb-1">Queue Status</label>
                <select id="statusFilter" bind:value={selectedStatus} class="px-3 py-2 rounded bg-zinc-900 border border-zinc-700 text-sm text-white focus:outline-none focus:border-teal-500 w-full">
                    <option value="">All Statuses</option>
                    <option value="Active">Active</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                </select>
            </div>
        </div>
    </div>

    {#if loading}
        <div class="text-center py-12 text-zinc-500">Loading statistics...</div>
    {:else if stats}
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <div class="p-6 rounded-xl border border-zinc-800 bg-zinc-900/50">
                <p class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Total Times Joined</p>
                <p class="text-3xl font-bold text-white mt-2">{stats.totalUsersJoined}</p>
            </div>
            <div class="p-6 rounded-xl border border-zinc-800 bg-zinc-900/50">
                <p class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Total Users Served</p>
                <p class="text-3xl font-bold text-teal-400 mt-2">{stats.totalUsersServed}</p>
            </div>
            <div class="p-6 rounded-xl border border-zinc-800 bg-zinc-900/50">
                <p class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Avg Wait Time</p>
                <p class="text-3xl font-bold text-white mt-2">{stats.averageWaitTimeMinutes} <span class="text-sm font-normal text-zinc-500">min</span></p>
            </div>
            <div class="p-6 rounded-xl border border-zinc-800 bg-zinc-900/50">
                <p class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Most Active</p>
                <p class="text-lg font-bold text-white mt-2 truncate" title={stats.mostActiveService}>{stats.mostActiveService}</p>
            </div>
        </div>
    {/if}

    <div class="pt-4 border-t border-zinc-800 flex justify-end">
        <button 
            on:click={handleDownload} 
            disabled={downloading || loading}
            class="px-6 py-3 rounded-lg font-semibold flex items-center gap-2 transition-colors disabled:opacity-50 hover:opacity-90 shadow-lg"
            style="background:#dc2626; color:white"
        >
            {#if downloading}
                Generating PDF...
            {:else}
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line><polyline points="10 9 9 9 8 9"></polyline></svg>
                Download PDF Report
            {/if}
        </button>
    </div>
</div>