<script lang="ts">
    import QueueCard from "$lib/components/queue-card.svelte";
    import ChatWidget from "$lib/components/chat-widget.svelte";
    import Search from "@lucide/svelte/icons/search";
    import TicketModal from "$lib/components/ticket-modal.svelte";
    import type { Queue, QueueEntry } from "$lib/types";

    interface PageData {
        user: { name: string; userId: number; token?: string };
        activeQueues: (QueueEntry & { queue: Queue })[];
        suggestedQueues: Queue[];
    }

    let { data }: { data: PageData } = $props();
    
    let searchQuery = $state("");
    let selectedEntry = $state<(QueueEntry & { queue: Queue }) | null>(null);
    let selectedQueue = $state<Queue | null>(null);
    let ticketModalOpen = $state(false);

    let filteredQueues = $derived(
        data.suggestedQueues.filter(
            (q: Queue) =>
                (q.name || "").toLowerCase().includes(searchQuery.toLowerCase()) ||
                (q.description || "").toLowerCase().includes(searchQuery.toLowerCase())
        )
    );
</script>

<div class="space-y-8 pb-12 font-['Inter']">
    <section>
        <h1 class="text-3xl font-bold tracking-tight text-[#1C2541]">
            Welcome back, {data.user.name || "User"}
        </h1>
        <p class="text-[#3A506B] mt-2 font-medium">
            Here is what's happening with your queues today.
        </p>
    </section>

    <section class="space-y-4">
        <h2 class="text-xl font-bold tracking-tight text-[#1C2541]">Active Queues</h2>
        {#if data.activeQueues.length === 0}
            <div class="rounded-[12px] border-2 border-dashed border-[#5BC0BE]/50 bg-[#5BC0BE]/5 p-8 text-center text-[#3A506B] font-medium">
                You are not currently in any queues.
            </div>
        {:else}
            <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                {#each data.activeQueues as entry}
                    <QueueCard
                        queue={entry.queue}
                        {entry}
                        section="active"
                        currentUserId={Number(data.user.userId)}
                        onviewticket={() => {
                            selectedEntry = entry;
                            selectedQueue = null;
                            ticketModalOpen = true;
                        }}
                    />
                {/each}
            </div>
        {/if}
    </section>

    <section class="space-y-4">
        <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <h2 class="text-xl font-bold tracking-tight text-[#1C2541]">Discover Queues</h2>
            <div class="relative w-full max-w-sm group">
                <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                    <Search class="h-5 w-5 text-[#3A506B]/50 group-focus-within:text-[#5BC0BE] transition-colors" />
                </div>
                <input
                    type="search"
                    placeholder="Find a service"
                    bind:value={searchQuery}
                    class="w-full bg-white border border-gray-200 shadow-sm rounded-[8px] pl-10 py-2.5 text-sm text-[#1C2541] placeholder:text-[#3A506B]/50 outline-none transition-all focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE]"
                />
            </div>
        </div>

        <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {#each filteredQueues as queue}
                {@const existingEntry = data.activeQueues.find(e => e.queue?.id === queue.id)}
                
                <QueueCard
                    {queue}
                    entry={existingEntry}
                    currentUserId={Number(data.user.userId)}
                    onviewticket={() => {
                        selectedQueue = queue;
                        selectedEntry = existingEntry || null;
                        ticketModalOpen = true;
                    }}
                />
            {/each}
        </div>
    </section>
</div>

<TicketModal
    open={ticketModalOpen}
    queue={selectedQueue || selectedEntry?.queue}
    entry={selectedEntry ?? undefined}
    onclose={() => {
        ticketModalOpen = false;
        selectedEntry = null;
        selectedQueue = null;
    }}
/>

<ChatWidget queues={data.suggestedQueues} userId={Number(data.user.userId)} />