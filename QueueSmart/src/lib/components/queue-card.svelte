<script lang="ts">
    import Button from "./button.svelte";
    import Badge, { type BadgeVariant } from "./badge.svelte";
    import type { Queue, QueueEntry, QueueStatus } from "$lib/types";
    import clsx from "clsx";
    import type { Component } from "svelte";
    import { joinQueue, leaveQueue } from "$lib/api/services"; 
    import { invalidateAll } from "$app/navigation";
    import {
        Building2Icon,
        CircleCheckBigIcon,
        InfoIcon,
        PauseIcon,
        PlusIcon,
        TicketIcon,
        XIcon,
        AlertCircleIcon
    } from "@lucide/svelte";

    let {
        queue,
        entry,
        section = "discoverable",
        onviewticket,
        currentUserId,
    }: {
        queue: Queue;
        entry?: QueueEntry; 
        section?: "active" | "discoverable";
        onviewticket?: (entryId: string) => void;
        currentUserId: number;
    } = $props();

    // Modal & Processing States
    let showLeaveModal = $state(false);
    let showJoinModal = $state(false);
    let joinWaitTime = $state(0);
    let actionError = $state<string | null>(null);
    let isProcessing = $state(false);
    let optimisticOffset = $state(0);
    
    let liveQueueLength = $derived(
        Math.max(
            (queue?.currentLength || 0) + optimisticOffset, 
            entry?.position || 0
        )
    );

    $effect(() => {
        queue?.currentLength; 
        optimisticOffset = 0; 
    });

    function getQueueStatusBadgeUI(status?: QueueStatus): {
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

    async function handleJoinClick() {
        if (!currentUserId) {
            actionError = "Error: You must be logged in to join a queue.";
            return;
        }

        isProcessing = true;
        try {
            const response = await joinQueue({
                userId: currentUserId,
                queueId: (queue as any).activeQueueId || queue.id, 
                priority: 1
            });
        
            joinWaitTime = response.estimatedWaitMinutes;
            optimisticOffset += 1; 
            showJoinModal = true;
            await invalidateAll(); 
            
        } catch (error: any) {
            console.error(error);
            actionError = error.message;
        } finally {
            isProcessing = false;
        }
    }

    async function confirmLeave() {
        if (!currentUserId || !entry?.id) return;

        isProcessing = true;
        try {
            await leaveQueue({
                userId: currentUserId,
                entryId: Number(entry.id) 
            });
            
            optimisticOffset -= 1; 
            showLeaveModal = false;
            await invalidateAll(); 
            
        } catch (error: any) {
            console.error(error);
            showLeaveModal = false;
            actionError = error.message;
        } finally {
            isProcessing = false;
        }
    }

    let { variant, icon } = $derived(getQueueStatusBadgeUI(queue?.status));
</script>

{#if queue}
    <div class={clsx("rounded-xl font-nunito bg-muted bg-card text-muted-foreground shadow-md shadow-accent/50 p-6 flex flex-col gap-4")}>
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
                <span class="font-semibold">{liveQueueLength} people</span>
            </div>
            {#if entry}
                <div class="flex flex-col bg-accent p-1.5 rounded-sm">
                    <span class="opacity-85 text-xs">Your Position</span>
                    <span class="font-semibold text-primary">#{entry.position}</span>
                </div>
            {/if}
        </div>

        <div class="mt-4 pt-4 border-t border-muted-foreground/15 flex justify-end">
            {#if section === "active" && entry}
                <div class="flex items-center gap-2">
                    <Button variant="destructive" size="sm" onclick={() => showLeaveModal = true} disabled={isProcessing}>
                        <XIcon size={18} /> Leave
                    </Button>
                    <Button variant="primary" size="sm" class="bg-primary/80 hover:bg-primary" onclick={() => onviewticket?.(entry.id.toString())}>
                        <TicketIcon size={18} /> View Ticket
                    </Button>
                </div>
            {:else}
                <Button
                    variant={queue.status === "open" && !entry ? "outline" : "ghost"}
                    size="sm"
                    onclick={handleJoinClick}
                    disabled={queue.status !== "open" || isProcessing || entry !== undefined}
                    class={queue.status === "open" && !entry ? "gap-1" : "text-muted-foreground opacity-60"}
                >
                    {#if queue.status === "open" && !isProcessing && !entry}
                        <PlusIcon size={18} />
                    {/if}
                    
                    {isProcessing 
                        ? "Processing..." 
                        : entry !== undefined 
                            ? "Already Joined" 
                            : queue.status === "open" 
                                ? "Join Queue" 
                                : "Unavailable"}
                </Button>
            {/if}
        </div>
    </div>

    {#if showLeaveModal || showJoinModal || actionError}
        <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
            
            <div class={clsx("rounded-xl font-nunito bg-muted bg-card text-muted-foreground shadow-lg shadow-accent/50 p-6 flex flex-col gap-4 w-full max-w-sm animate-in fade-in zoom-in-95 duration-200")}>
                
                {#if showLeaveModal}
                    <div class="flex flex-col items-center text-center gap-2">
                        <XIcon size={32} class="opacity-80 mb-2" />
                        <h3 class="font-semibold leading-none text-lg tracking-tight">Leave Queue?</h3>
                        <p class="text-sm opacity-80">Are you sure you want to leave the queue for <strong>{queue.name}</strong>?</p>
                    </div>
                    <div class="mt-2 flex justify-center gap-3">
                        <Button variant="outline" size="sm" onclick={() => showLeaveModal = false} disabled={isProcessing}>
                            Cancel
                        </Button>
                        <Button variant="destructive" size="sm" onclick={confirmLeave} disabled={isProcessing}>
                            {isProcessing ? "Leaving..." : "Yes, Leave"}
                        </Button>
                    </div>

                {:else if showJoinModal}
                    <div class="flex flex-col items-center text-center gap-2">
                        <CircleCheckBigIcon size={32} class="opacity-80 mb-2" />
                        <h3 class="font-semibold leading-none text-lg tracking-tight">Joined Successfully!</h3>
                        <p class="text-sm opacity-80">You are now in line for <strong>{queue.name}</strong>.</p>
                        
                        <div class="flex flex-col bg-accent p-2 rounded-sm w-full mt-2 text-center">
                            <span class="opacity-85 text-xs">Estimated Wait Time</span>
                            <span class="font-semibold text-lg">{joinWaitTime} minutes</span>
                        </div>
                    </div>
                    <div class="mt-2 flex justify-center">
                        <Button variant="outline" size="sm" class="w-full" onclick={() => showJoinModal = false}>
                            Close
                        </Button>
                    </div>

                {:else if actionError}
                    <div class="flex flex-col items-center text-center gap-2">
                        <AlertCircleIcon size={32} class="opacity-80 mb-2" />
                        <h3 class="font-semibold leading-none text-lg tracking-tight">Something went wrong</h3>
                        <p class="text-sm opacity-80">{actionError}</p>
                    </div>
                    <div class="mt-2 flex justify-center">
                        <Button variant="outline" size="sm" class="w-full" onclick={() => actionError = null}>
                            Dismiss
                        </Button>
                    </div>
                {/if}

            </div>
        </div>
    {/if}
{/if}