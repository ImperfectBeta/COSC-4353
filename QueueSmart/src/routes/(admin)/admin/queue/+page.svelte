<script lang="ts">
    import { 
        services, 
        activeQueueEntries, 
        activeQueueId,
        loadQueueEntries,
        processNextUser,
        dropUserFromQueue,
        saveQueueOrder
    } from "$lib/stores/adminStore";

    let selectedServiceId: string | null = null;

    $: svc = $services ?? [];
    $: currentQueue = $activeQueueEntries ?? [];
    $: currentQId = $activeQueueId;
    $: selectedService = svc.find(s => s.id === selectedServiceId);

    $: if (selectedServiceId) {
        loadQueueEntries(selectedServiceId);
    }

    async function serveNext() {
        if (!currentQId || currentQueue.length === 0) return;
        await processNextUser(currentQId);
    }

    async function removeUser(entryId: number, userId: number) {
        if (!currentQId) return;
        if (confirm("Are you sure you want to manually remove this user from the queue?")) {
            await dropUserFromQueue(currentQId, entryId, userId);
        }
    }

    function formatTime(isoString: string) {
        if (!isoString) return "Unknown time";
        const date = new Date(isoString);
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    function moveUp(index: number) {
        if (selectedServiceId === null || index === 0 || !currentQId) return;
        activeQueueEntries.update(q => {
            const arr = [...q];
            [arr[index - 1], arr[index]] = [arr[index], arr[index - 1]];
            saveQueueOrder(currentQId, arr);
            return arr;
        });
    }

    function moveDown(index: number) {
        if (selectedServiceId === null || !currentQId) return;
        activeQueueEntries.update(q => {
            const arr = [...q];
            if (index >= arr.length - 1) return q;
            [arr[index], arr[index + 1]] = [arr[index + 1], arr[index]];
            saveQueueOrder(currentQId, arr);
            return arr;
        });
    }
</script>

<div class="p-8 space-y-8">
    <div>
        <h2 class="text-2xl font-bold text-background">Queue Management</h2>
        <p class="text-sm mt-1 text-background">
            View and manage live queues for each service.
        </p>
    </div>

    <div class="flex gap-3 flex-wrap">
        {#each $services as service}
            <button
                on:click={() => (selectedServiceId = service.id)}
                class="px-4 py-2 rounded-lg cursor-pointer text-sm transition-colors"
                style={selectedServiceId === service.id
                    ? "background:rgba(91,192,190,0.1); color:var(--color-primary); border:1px solid rgba(91,192,190,0.3)"
                    : "border:1px solid transparent; color:var(--color-background); opacity: 0.7"}
            >
                {service.name}
            </button>
        {/each}
    </div>

    {#if selectedService}
	<!-- Queue Panel -->
        <div class="rounded-xl overflow-hidden" style="background:var(--color-muted); border:1px solid rgba(0,0,0,0.12)">
            <div class="px-6 py-4" style="border-bottom:1px solid rgba(0,0,0,0.12); display:flex; align-items:center; justify-content:space-between">
                <div>
                    <h3 class="font-semibold text-white">{selectedService.name}</h3>
                    <p class="text-navy-600 text-xs mt-0.5">
                        {currentQueue.length} user{currentQueue.length !== 1 ? "s" : ""} waiting
                    </p>
                </div>
                <button
                    on:click={serveNext}
                    disabled={currentQueue.length === 0}
                    class="px-4 py-2 text-sm cursor-pointer rounded-lg font-semibold transition-all"
                    style={currentQueue.length > 0
                        ? "background:var(--color-primary); color:var(--color-foreground)"
                        : "background:rgba(0,0,0,0.06); color:var(--color-background); cursor:not-allowed; opacity: 0.5"}
                >
                    Serve Next ›
                </button>
            </div>

            {#if currentQueue.length === 0}
                <div class="px-6 py-12 text-center text-navy-600 text-sm">
                    Queue is empty. Waiting for users to join...
                </div>
            {:else}
                <ul>
                    {#each currentQueue as user, i (user.id)}
                        <li class="px-6 py-4 flex items-center gap-4 border-b border-black/10 last:border-0" style="list-style:none; display:flex; align-items:center; gap:1rem; padding:1rem 1.5rem">
                            <span class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0"
                                style={i === 0
                                    ? "background:rgba(91,192,190,0.2); color:var(--color-primary); border:1px solid rgba(91,192,190,0.3)"
                                    : "background:rgba(0,0,0,0.06); color:var(--color-background); border:1px solid rgba(0,0,0,0.12)"}
                            >
                                {user.position || (i + 1)}
                            </span>
                            <div class="flex-1 min-w-0">
                                <p class="text-sm font-medium" style="color:var(--color-primary-foreground)">{user.userName}</p>
                                <p class="text-xs" style="color:var(--color-background)">Joined at {formatTime(user.joinTime)} • {user.estimatedWaitMinutes} min wait</p>
                            </div>
                            <div class="flex items-center gap-1 flex-shrink-0">
                                <button on:click={() => moveUp(i)} disabled={i === 0} class="w-7 h-7 rounded flex items-center justify-center text-xs disabled:opacity-30 transition-opacity" style="color:var(--color-background); background:transparent; border: none; cursor:pointer;">▲</button>
                                <button on:click={() => moveDown(i)} disabled={i === currentQueue.length - 1} class="w-7 h-7 rounded flex items-center justify-center text-xs disabled:opacity-30 transition-opacity" style="color:var(--color-background); background:transparent; border: none; cursor:pointer;">▼</button>
                                <button on:click={() => removeUser(user.id, user.userId)} class="w-7 h-7 rounded flex items-center justify-center text-xs ml-1 hover:bg-red-500/10 transition-colors" style="color:#f87171; background:transparent; border:none; cursor:pointer;" title="Remove User">✕</button>
                            </div>
                        </li>
                    {/each}
                </ul>
            {/if}
        </div>
    {:else}
        <div class="bg-zinc-900 border border-zinc-800 rounded-xl px-6 py-12 text-center text-zinc-600 text-sm">
            Select a service above to view its queue.
        </div>
    {/if}
</div>