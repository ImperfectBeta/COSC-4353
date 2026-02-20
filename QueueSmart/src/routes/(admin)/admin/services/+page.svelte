<script lang="ts">
  import { services } from '$lib/stores/adminStore';

  interface Service {
    id: number;
    name: string;
    description: string;
    duration: number;
    priority: string;
    isOpen: boolean;
    queueLength: number;
  }

  interface FormData {
    name: string;
    description: string;
    duration: string | number;
    priority: string;
  }

  let showForm = false;
  let editingId: number | null = null;

  const empty = (): FormData => ({ name: '', description: '', duration: '', priority: 'medium' });
  let form: FormData = empty();
  let errors: Record<string, string> = {};

  $: svc = $services ?? [];

  function openCreate() {
    form = empty();
    editingId = null;
    errors = {};
    showForm = true;
  }

  function openEdit(service: Service) {
    form = { ...service, duration: service.duration };
    editingId = service.id;
    errors = {};
    showForm = true;
  }

  function validate() {
    errors = {};
    if (!form.name.trim())                        errors.name = 'Service name is required.';
    else if (form.name.length > 100)              errors.name = 'Max 100 characters.';
    if (!form.description.trim())                 errors.description = 'Description is required.';
    const dur = typeof form.duration === 'string' ? parseInt(form.duration) : form.duration;
    if (!dur || dur <= 0)                         errors.duration = 'Must be a positive number.';
    return Object.keys(errors).length === 0;
  }

  function save() {
    if (!validate()) return;

    services.update(list => {
      if (editingId) {
        return (list || []).map(s => s.id === editingId ? { ...s, ...form, duration: +form.duration } : s);
      } else {
        const newService = {
          ...form,
          id: Date.now(),
          duration: +form.duration,
          isOpen: true,
          queueLength: 0,
        };
        return [...(list || []), newService];
      }
    });

    showForm = false;
  }

  function deleteService(id: number) {
    if (confirm('Delete this service?')) {
      services.update(list => (list || []).filter(s => s.id !== id));
    }
  }

  const priorityColor = {
    high:   'text-red-500 bg-red-500/10 border-red-500/20',
    medium: 'text-yellow-400 bg-yellow-500/10 border-yellow-500/20',
    low:    'text-teal bg-teal/10 border-teal/20',
  };
</script>

<div class="p-8 space-y-8">
  <div class="flex justify-between items-start">
    <div>
      <h2 class="text-2xl font-bold text-teal">Service Management</h2>
      <p class="text-navy-600 text-sm mt-1">Create, edit, and manage available services.</p>
    </div>
    <button
      on:click={openCreate}
      class="text-sm font-semibold px-4 py-2 rounded-lg"
      style="background:var(--color-primary); color:var(--color-foreground); transition:background .15s">
      + New Service
    </button>
  </div>

  <!-- Service Cards -->
  <div class="grid grid-cols-1 gap-4">
    {#each svc as service (service.id)}
      <div class="rounded-xl p-5 flex items-center gap-4" style="background:var(--color-muted); border:1px solid rgba(0,0,0,0.12)">
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-3">
            <h3 class="font-semibold" style="color:var(--color-primary-foreground)">{service.name}</h3>
            <span class="text-xs px-2 py-0.5 rounded" style={
              priorityColor[service.priority as keyof typeof priorityColor] === 'text-teal bg-teal/10 border-teal/20'
                ? 'color:var(--color-primary); background:rgba(91,192,190,0.1); border:1px solid rgba(91,192,190,0.2)'
                : priorityColor[service.priority as keyof typeof priorityColor] === 'text-red-500 bg-red-500/10 border-red-500/20'
                  ? 'color:#ef4444; background: rgba(239,68,68,0.1); border:1px solid rgba(239,68,68,0.2)'
                  : 'color:#fbbf24; background: rgba(245,158,11,0.1); border:1px solid rgba(245,158,11,0.2)'
            }>
              {service.priority}
            </span>
            <span class="text-xs px-2 py-0.5 rounded" style={service.isOpen ? 'color:var(--color-primary); background:rgba(91,192,190,0.1); border:1px solid rgba(91,192,190,0.2)' : 'color:var(--color-background); background:rgba(0,0,0,0.06); border:1px solid rgba(0,0,0,0.12)'}>
              {service.isOpen ? 'Open' : 'Closed'}
            </span>
          </div>
          <p class="text-sm mt-1 truncate" style="color:var(--color-background)">{service.description}</p>
          <p class="text-xs mt-1" style="color:var(--color-background)">⏱ {service.duration} min per person</p>
        </div>
        <div class="flex gap-2 flex-shrink-0">
          <button
            on:click={() => openEdit(service)}
            class="text-xs px-3 py-1.5 rounded"
            style="border:1px solid rgba(0,0,0,0.12); color:var(--color-primary); background:transparent">
            Edit
          </button>
          <button
            on:click={() => deleteService(service.id)}
            class="text-xs px-3 py-1.5 rounded"
            style="border:1px solid rgba(239,68,68,0.2); color:#f87171; background:transparent">
            Delete
          </button>
        </div>
      </div>
    {/each}
  </div>
</div>

<!-- Modal -->
{#if showForm}
  <!-- Backdrop -->
  <div
    class="fixed inset-0 bg-black/60 backdrop-blur-sm z-40"
    on:click={() => showForm = false}
    role="presentation"
  ></div>

  <!-- Modal Panel -->
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4">
    <div class="bg-navy-800 border border-navy-600 rounded-2xl w-full max-w-lg shadow-2xl">
      <div class="px-6 py-5 border-b border-navy-600 flex justify-between items-center">
        <h3 class="font-semibold text-white">
          {editingId ? 'Edit Service' : 'Create New Service'}
        </h3>
        <button on:click={() => showForm = false}
          class="text-navy-600 hover:text-teal transition-colors text-xl leading-none">
          ✕
        </button>
      </div>

      <div class="p-6 space-y-4">
        <!-- Name -->
        <div>
          <label class="block text-xs text-navy-600 uppercase tracking-wider mb-1.5">
            Service Name <span class="text-red-400">*</span>
          </label>
          <input
            bind:value={form.name}
            maxlength="100"
            placeholder="e.g. General Consultation"
            class="w-full bg-navy-700 border rounded-lg px-3 py-2.5 text-sm text-white
                   placeholder:text-navy-500 outline-none transition-colors
                   focus:border-teal
                   {errors.name ? 'border-red-500' : 'border-navy-600'}"
          />
          {#if errors.name}
            <p class="text-red-400 text-xs mt-1">{errors.name}</p>
          {/if}
          <p class="text-navy-600 text-xs mt-1">{form.name.length}/100</p>
        </div>

        <!-- Description -->
        <div>
          <label class="block text-xs text-navy-600 uppercase tracking-wider mb-1.5">
            Description <span class="text-red-400">*</span>
          </label>
          <textarea
            bind:value={form.description}
            rows="3"
            placeholder="Briefly describe this service..."
            class="w-full bg-navy-700 border rounded-lg px-3 py-2.5 text-sm text-white
                   placeholder:text-navy-500 outline-none resize-none transition-colors
                   focus:border-teal
                   {errors.description ? 'border-red-500' : 'border-navy-600'}"
          ></textarea>
          {#if errors.description}
            <p class="text-red-400 text-xs mt-1">{errors.description}</p>
          {/if}
        </div>

        <!-- Duration + Priority -->
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs text-navy-600 uppercase tracking-wider mb-1.5">
              Duration (min) <span class="text-red-400">*</span>
            </label>
            <input
              type="number"
              bind:value={form.duration}
              min="1"
              placeholder="15"
              class="w-full bg-navy-700 border rounded-lg px-3 py-2.5 text-sm text-white
                     placeholder:text-navy-500 outline-none transition-colors
                     focus:border-teal
                     {errors.duration ? 'border-red-500' : 'border-navy-600'}"
            />
            {#if errors.duration}
              <p class="text-red-400 text-xs mt-1">{errors.duration}</p>
            {/if}
          </div>

          <div>
            <label class="block text-xs text-navy-600 uppercase tracking-wider mb-1.5">
              Priority Level
            </label>
            <select
              bind:value={form.priority}
              class="w-full bg-navy-700 border border-navy-600 rounded-lg px-3 py-2.5
                     text-sm text-white outline-none focus:border-teal transition-colors">
              <option value="low">Low</option>
              <option value="medium">Medium</option>
              <option value="high">High</option>
            </select>
          </div>
        </div>
      </div>

      <div class="px-6 py-4 border-t border-navy-600 flex justify-end gap-3">
        <button
          on:click={() => showForm = false}
          class="px-4 py-2 text-sm rounded-lg border border-navy-600 text-zinc-400
                 hover:bg-navy-700 transition-colors">
          Cancel
        </button>
        <button
          on:click={save}
          class="px-4 py-2 text-sm rounded-lg bg-teal hover:bg-teal/80
                 text-navy-900 font-semibold transition-colors">
          {editingId ? 'Save Changes' : 'Create Service'}
        </button>
      </div>
    </div>
  </div>
{/if}
