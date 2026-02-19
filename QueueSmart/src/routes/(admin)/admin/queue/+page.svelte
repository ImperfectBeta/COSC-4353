<script lang="ts">
  import { services, queues } from '$lib/stores/adminStore';

  interface QueueUser {
    id: number;
    name: string;
    joinedAt: string;
    status: string;
  }

  let selectedServiceId: number | null = null;

  $: svc = $services ?? [];
  $: qmap = ($queues ?? {}) as Record<number, QueueUser[]>;

  $: selectedService = svc.find(s => s.id === selectedServiceId);
  $: currentQueue = selectedServiceId && selectedServiceId in qmap ? (qmap[selectedServiceId] ?? []) : [];

  function serveNext() {
    if (selectedServiceId === null || currentQueue.length === 0) return;
    queues.update(q => {
      const id = selectedServiceId as number;
      const updated = [...((q as any)[id] ?? [])];
      updated.shift();
      return { ...q, [id]: updated };
    });
    services.update(list =>
      (list || []).map(s => s.id === selectedServiceId
        ? { ...s, queueLength: Math.max(0, s.queueLength - 1) }
        : s
      )
    );
  }

  function removeUser(userId: number) {
    if (selectedServiceId === null) return;
    const id = selectedServiceId as number & {};
    queues.update(q => ({
      ...q,
      [id]: ((q as any)[id] ?? []).filter((u: QueueUser) => u.id !== userId)
    }));
    services.update(list =>
      (list || []).map(s => s.id === selectedServiceId
        ? { ...s, queueLength: Math.max(0, s.queueLength - 1) }
        : s
      )
    );
  }

  function moveUp(index: number) {
    if (selectedServiceId === null || index === 0) return;
    queues.update(q => {
      const id = selectedServiceId as number;
      const arr = [...((q as any)[id] ?? [])];
      [arr[index - 1], arr[index]] = [arr[index], arr[index - 1]];
      return { ...q, [id]: arr };
    });
  }

  function moveDown(index: number) {
    if (selectedServiceId === null) return;
    queues.update(q => {
      const id = selectedServiceId as number;
      const arr = [...((q as any)[id] ?? [])];
      if (index >= arr.length - 1) return q;
      [arr[index], arr[index + 1]] = [arr[index + 1], arr[index]];
      return { ...q, [id]: arr };
    });
  }
</script>

  <div class="p-8 space-y-8">
  <div>
    <h2 class="text-2xl font-bold" style="color:var(--color-primary)">Queue Management</h2>
    <p class="text-sm mt-1" style="color:var(--color-background)">View and manage live queues for each service.</p>
  </div>

  <!-- Service Selector -->
  <div class="flex gap-3 flex-wrap">
    {#each $services as service}
      <button
        on:click={() => selectedServiceId = service.id}
        class="px-4 py-2 rounded-lg text-sm"
        style={selectedServiceId === service.id ? 'background:rgba(91,192,190,0.1); color:var(--color-primary); border:1px solid rgba(91,192,190,0.3)' : 'border:1px solid transparent; color:var(--color-background)'}>
        {service.name}
        <span class="ml-2 text-xs opacity-70">{service.queueLength}</span>
      </button>
    {/each}
  </div>

  {#if selectedService}
    <!-- Queue Panel -->
    <div class="rounded-xl overflow-hidden" style="background:var(--color-muted-foreground); border:1px solid rgba(0,0,0,0.12)">
      <div class="px-6 py-4" style="border-bottom:1px solid rgba(0,0,0,0.12); display:flex; align-items:center; justify-content:space-between">
        <div>
          <h3 class="font-semibold text-white">{selectedService.name}</h3>
          <p class="text-navy-600 text-xs mt-0.5">
            {currentQueue.length} user{currentQueue.length !== 1 ? 's' : ''} waiting
          </p>
        </div>
        <button
          on:click={serveNext}
          disabled={currentQueue.length === 0}
          class="px-4 py-2 text-sm rounded-lg font-semibold"
          style={currentQueue.length > 0 ? 'background:var(--color-primary); color:var(--color-foreground)' : 'background:rgba(0,0,0,0.06); color:var(--color-background); cursor:not-allowed'}>
          Serve Next ›
        </button>
      </div>

      {#if currentQueue.length === 0}
        <div class="px-6 py-12 text-center text-navy-600 text-sm">
          Queue is empty.
        </div>
      {:else}
        <ul>
          {#each currentQueue as user, i (user.id)}
            <li class="px-6 py-4 flex items-center gap-4" style="list-style:none; display:flex; align-items:center; gap:1rem; padding:1rem 1.5rem">
              <!-- Position badge -->
              <span class="w-7 h-7 rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0" style={i === 0 ? 'background:rgba(91,192,190,0.2); color:var(--color-primary); border:1px solid rgba(91,192,190,0.3)' : 'background:rgba(0,0,0,0.06); color:var(--color-background); border:1px solid rgba(0,0,0,0.12)'}>
                {i + 1}
              </span>

              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium" style="color:var(--color-primary-foreground)">{user.name}</p>
                <p class="text-xs" style="color:var(--color-background)">Joined at {user.joinedAt}</p>
              </div>

              <!-- Reorder + Remove -->
              <div class="flex items-center gap-1 flex-shrink-0">
                <button
                  on:click={() => moveUp(i)}
                  disabled={i === 0}
                  class="w-7 h-7 rounded flex items-center justify-center text-xs"
                  style="color:var(--color-background); background:transparent; border: none">
                  ▲
                </button>
                <button
                  on:click={() => moveDown(i)}
                  disabled={i === currentQueue.length - 1}
                  class="w-7 h-7 rounded flex items-center justify-center text-xs"
                  style="color:var(--color-background); background:transparent; border: none">
                  ▼
                </button>
                <button
                  on:click={() => removeUser(user.id)}
                  class="w-7 h-7 rounded flex items-center justify-center text-xs ml-1"
                  style="color:#f87171; background:transparent; border:none">
                  ✕
                </button>
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
