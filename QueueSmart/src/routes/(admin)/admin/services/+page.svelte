<script lang="ts">
    import { onMount } from "svelte";
    import {
        services,
        servicesLoading,
        servicesError,
        loadServices,
        addService,
        editService,
        removeService,
    } from "$lib/stores/adminStore";

    interface FormData {
        name: string;
        description: string;
        duration: string | number;
        priority: string;
    }

    let showForm = false;
    let editingId: string | null = null;
    let saving = false;
    let apiError = "";

    const empty = (): FormData => ({
        name: "",
        description: "",
        duration: "",
        priority: "medium",
    });
    let form: FormData = empty();
    let errors: Record<string, string> = {};

    $: svc = $services ?? [];

    onMount(() => {
        loadServices();
    });

    function openCreate() {
        form = empty();
        editingId = null;
        errors = {};
        apiError = "";
        showForm = true;
    }

    function openEdit(service: any) {
        form = {
            name: service.name,
            description: service.description,
            duration: service.duration,
            priority: service.priority,
        };
        editingId = service.id;
        errors = {};
        apiError = "";
        showForm = true;
    }

    function validate() {
        errors = {};
        if (!form.name.trim()) errors.name = "Service name is required.";
        else if (form.name.length > 100) errors.name = "Max 100 characters.";
        if (!form.description.trim())
            errors.description = "Description is required.";
        const dur =
            typeof form.duration === "string"
                ? parseInt(form.duration)
                : form.duration;
        if (!dur || dur <= 0) errors.duration = "Must be a positive number.";
        return Object.keys(errors).length === 0;
    }

    async function save() {
        if (!validate()) return;
        saving = true;
        apiError = "";

        const data = {
            name: form.name,
            description: form.description,
            duration:
                typeof form.duration === "string"
                    ? parseInt(form.duration)
                    : form.duration,
            priority: form.priority,
        };

        try {
            if (editingId) {
                await editService(editingId, data);
            } else {
                await addService(data);
            }
            showForm = false;
        } catch (err: any) {
            apiError = err.message || "Failed to save service.";
        } finally {
            saving = false;
        }
    }

    async function deleteService(id: string) {
        if (!confirm("Delete this service?")) return;
        try {
            await removeService(id);
        } catch (err: any) {
            alert(err.message || "Failed to delete service.");
        }
    }

    const priorityColor: Record<string, string> = {
        high: "text-red-400 bg-red-500/20 border-red-500/30",
        medium: "text-yellow-400 bg-yellow-500/20 border-yellow-500/30",
        low: "text-[#5BC0BE] bg-[#5BC0BE]/20 border-[#5BC0BE]/30",
    };
</script>

<div class="p-8 md:p-10 space-y-8 max-w-7xl mx-auto font-['Inter']">
    <div class="flex justify-between items-start">
        <div>
            <h2 class="text-3xl font-bold text-white">Service Management</h2>
            <p class="text-white/60 text-sm mt-2">
                Create, edit, and manage available services for your organization.
            </p>
        </div>
        <button
            on:click={openCreate}
            class="text-sm font-bold px-5 py-2.5 rounded-[8px] cursor-pointer bg-[#5BC0BE] text-[#1C2541] transition-all hover:bg-[#4aa8a6] hover:shadow-lg hover:-translate-y-0.5"
        >
            + New Service
        </button>
    </div>

    {#if $servicesError}
        <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-[12px] p-4 text-sm font-medium">
            {$servicesError}
        </div>
    {/if}

    {#if $servicesLoading}
        <div class="flex items-center gap-3 text-[#5BC0BE] text-sm font-medium">
            <svg class="animate-spin h-5 w-5" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Loading services...
        </div>
    {:else}
    <!-- Service Cards -->    
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            {#each svc as service (service.id)}
                <div class="rounded-[16px] p-6 flex flex-col justify-between bg-[#1C2541] border border-white/10 shadow-md hover:border-white/20 transition-colors">
                    
                    <div class="mb-6">
                        <div class="flex items-start justify-between gap-3 mb-2">
                            <h3 class="font-bold text-lg text-white">
                                {service.name}
                            </h3>
                            <div class="flex flex-col items-end gap-2 flex-shrink-0">
                                <span class={`text-xs px-2.5 py-1 font-medium rounded-[6px] border ${priorityColor[service.priority] ?? ""}`}>
                                    {service.priority}
                                </span>
                                <span class={`text-xs px-2.5 py-1 font-medium rounded-[6px] border ${
                                    service.isOpen
                                        ? "text-[#5BC0BE] bg-[#5BC0BE]/10 border-[#5BC0BE]/30"
                                        : "text-gray-400 bg-gray-500/20 border-gray-500/30"
                                }`}>
                                    {service.isOpen ? "Open" : "Closed"}
                                </span>
                            </div>
                        </div>
                        <p class="text-sm text-white/70 leading-relaxed">
                            {service.description}
                        </p>
                    </div>

                    <div class="flex items-center justify-between mt-auto pt-4 border-t border-white/10">
                        <p class="text-xs font-medium text-[#5BC0BE] bg-[#5BC0BE]/10 px-3 py-1.5 rounded-[6px]">
                            ⏱ {service.duration} min / person
                        </p>
                        <div class="flex gap-2">
                            <button
                                on:click={() => openEdit(service)}
                                class="text-xs font-semibold px-4 py-2 rounded-[6px] cursor-pointer bg-white/5 border border-white/10 text-white hover:bg-white/10 transition-colors"
                            >
                                Edit
                            </button>
                            <button
                                on:click={() => deleteService(service.id)}
                                class="text-xs font-semibold px-4 py-2 rounded-[6px] cursor-pointer bg-red-500/10 border border-red-500/20 text-red-400 hover:bg-red-500/20 transition-colors"
                            >
                                Delete
                            </button>
                        </div>
                    </div>

                </div>
            {/each}
        </div>
    {/if}
</div>

<!-- Modal -->
{#if showForm}
    <!-- Backdrop -->
    <div
        class="fixed inset-0 bg-[#0B132B]/80 backdrop-blur-sm z-40"
        on:click={() => (showForm = false)}
        on:keydown={(e) => e.key === 'Escape' && (showForm = false)}
        role="presentation"
    ></div>

<!-- Modal Panel -->    
<div class="fixed inset-0 z-50 flex items-center justify-center p-4 font-['Inter']">
        <div class="bg-[#1C2541] border border-white/10 rounded-[16px] w-full max-w-lg shadow-2xl overflow-hidden">
            
            <div class="px-8 py-5 border-b border-white/10 flex justify-between items-center bg-[#1C2541]">
                <h3 class="font-bold text-lg text-white">
                    {editingId ? "Edit Service" : "Create New Service"}
                </h3>
                <button
                    on:click={() => (showForm = false)}
                    class="text-white/50 hover:text-white transition-colors text-xl leading-none cursor-pointer"
                    aria-label="Close form"
                >
                    ✕
                </button>
            </div>

            <div class="p-8 space-y-5 bg-[#1C2541]">
                {#if apiError}
                    <div class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-[8px] p-3 text-sm font-medium">
                        {apiError}
                    </div>
                {/if}

                <!-- Name -->
                <div>
                    <label for="service-name" class="block text-xs text-white/60 font-semibold uppercase tracking-wider mb-2">
                        Service Name <span class="text-red-400">*</span>
                    </label>
                    <input
                        id="service-name"
                        bind:value={form.name}
                        maxlength="100"
                        placeholder="e.g. General Consultation"
                        class="w-full bg-[#0B132B] border rounded-[8px] px-4 py-3 text-sm text-white placeholder:text-white/30 outline-none transition-colors focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] {errors.name ? 'border-red-400' : 'border-white/20'}"
                    />
                    {#if errors.name}
                        <p class="text-red-400 text-xs mt-1.5 font-medium">{errors.name}</p>
                    {/if}
                    <p class="text-white/40 text-xs mt-1.5 text-right">{form.name.length}/100</p>
                </div>

                <!-- Description -->
                <div>
                    <label for="service-desc" class="block text-xs text-white/60 font-semibold uppercase tracking-wider mb-2">
                        Description <span class="text-red-400">*</span>
                    </label>
                    <textarea
                        id="service-desc"
                        bind:value={form.description}
                        rows="3"
                        placeholder="Briefly describe this service..."
                        class="w-full bg-[#0B132B] border rounded-[8px] px-4 py-3 text-sm text-white placeholder:text-white/30 outline-none resize-none transition-colors focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] {errors.description ? 'border-red-400' : 'border-white/20'}"
                    ></textarea>
                    {#if errors.description}
                        <p class="text-red-400 text-xs mt-1.5 font-medium">{errors.description}</p>
                    {/if}
                </div>

                <!-- Duration + Priority -->
                <div class="grid grid-cols-2 gap-5">
                    <div>
                        <label for="service-duration" class="block text-xs text-white/60 font-semibold uppercase tracking-wider mb-2">
                            Duration (min) <span class="text-red-400">*</span>
                        </label>
                        <input
                            id="service-duration"
                            type="number"
                            bind:value={form.duration}
                            min="1"
                            placeholder="15"
                            class="w-full bg-[#0B132B] border rounded-[8px] px-4 py-3 text-sm text-white placeholder:text-white/30 outline-none transition-colors focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] {errors.duration ? 'border-red-400' : 'border-white/20'}"
                        />
                        {#if errors.duration}
                            <p class="text-red-400 text-xs mt-1.5 font-medium">{errors.duration}</p>
                        {/if}
                    </div>

                    <div>
                        <label for="service-priority" class="block text-xs text-white/60 font-semibold uppercase tracking-wider mb-2">
                            Priority Level
                        </label>
                        <select
                            id="service-priority"
                            bind:value={form.priority}
                            class="w-full bg-[#0B132B] border border-white/20 rounded-[8px] px-4 py-3 text-sm text-white outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] transition-colors"
                        >
                            <option value="low">Low</option>
                            <option value="medium">Medium</option>
                            <option value="high">High</option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="px-8 py-5 border-t border-white/10 flex justify-end gap-3 bg-[#1C2541]">
                <button
                    on:click={() => (showForm = false)}
                    class="px-5 py-2.5 text-sm font-semibold rounded-[8px] cursor-pointer border border-white/20 text-white hover:bg-white/5 transition-colors"
                >
                    Cancel
                </button>
                <button
                    on:click={save}
                    disabled={saving}
                    class="px-5 py-2.5 text-sm font-bold rounded-[8px] cursor-pointer bg-[#5BC0BE] hover:bg-[#4aa8a6] text-[#1C2541] transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    {#if saving}
                        Saving...
                    {:else}
                        {editingId ? "Save Changes" : "Create Service"}
                    {/if}
                </button>
            </div>

        </div>
    </div>
{/if}