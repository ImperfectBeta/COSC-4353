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
		high: "text-red-500 bg-red-500/10 border border-red-500/20",
		medium: "text-yellow-400 bg-yellow-500/10 border border-yellow-500/20",
		low: "text-primary bg-primary/10 border border-primary/20",
	};
</script>

<div class="p-8 space-y-8">
	<div class="flex justify-between items-start">
		<div>
			<h2 class="text-2xl font-bold text-background">
				Service Management
			</h2>
			<p class="text-background text-sm mt-1">
				Create, edit, and manage available services bro!
			</p>
		</div>
		<button
			on:click={openCreate}
			class="text-sm font-semibold px-4 py-2 rounded-lg cursor-pointer bg-primary text-foreground transition-colors hover:bg-primary/80"
		>
			+ New Service
		</button>
	</div>

	{#if $servicesError}
		<div
			class="bg-destructive/10 border border-destructive/20 text-destructive rounded-lg p-4 text-sm"
		>
			{$servicesError}
		</div>
	{/if}

	{#if $servicesLoading}
		<p class="text-accent text-sm">Loading services...</p>
	{:else}
		<!-- Service Cards -->
		<div class="grid grid-cols-1 gap-4">
			{#each svc as service (service.id)}
				<div
					class="rounded-xl p-5 flex items-center gap-4 bg-muted border border-primary/20"
				>
					<div class="flex-1 min-w-0">
						<div class="flex items-center gap-3">
							<h3 class="font-semibold text-primary-foreground">
								{service.name}
							</h3>
							<span
								class={`text-xs px-2 py-0.5 rounded ${priorityColor[service.priority] ?? ""}`}
							>
								{service.priority}
							</span>
							<span
								class={`text-xs px-2 py-0.5 rounded border ${
									service.isOpen
										? "text-primary bg-primary/10 border-primary/20"
										: "text-accent bg-accent/25 border-accent/50"
								}`}
							>
								{service.isOpen ? "Open" : "Closed"}
							</span>
						</div>
						<p class="text-sm mt-1 truncate text-background">
							{service.description}
						</p>
						<p class="text-xs mt-1 text-background">
							⏱ {service.duration} min per person
						</p>
					</div>
					<div class="flex gap-2 flex-shrink-0">
						<button
							on:click={() => openEdit(service)}
							class="text-xs px-3 py-1.5 rounded cursor-pointer border border-primary/20 text-primary bg-transparent hover:bg-primary/10 transition-colors"
						>
							Edit
						</button>
						<button
							on:click={() => deleteService(service.id)}
							class="text-xs px-3 py-1.5 rounded cursor-pointer border border-destructive/20 text-destructive bg-transparent hover:bg-destructive/10 transition-colors"
						>
							Delete
						</button>
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
		class="fixed inset-0 bg-black/60 backdrop-blur-sm z-40"
		on:click={() => (showForm = false)}
		role="presentation"
	></div>

	<!-- Modal Panel -->
	<div class="fixed inset-0 z-50 flex items-center justify-center p-4">
		<div
			class="bg-muted border border-primary/20 rounded-2xl w-full max-w-lg shadow-2xl"
		>
			<div
				class="px-6 py-5 border-b border-primary/20 flex justify-between items-center"
			>
				<h3 class="font-semibold text-primary-foreground">
					{editingId ? "Edit Service" : "Create New Service"}
				</h3>
				<button
					on:click={() => (showForm = false)}
					class="text-accent hover:text-primary transition-colors text-xl leading-none cursor-pointer"
				>
					✕
				</button>
			</div>

			<div class="p-6 space-y-4">
				{#if apiError}
					<div
						class="bg-destructive/10 border border-destructive/20 text-destructive rounded-lg p-3 text-sm"
					>
						{apiError}
					</div>
				{/if}

				<!-- Name -->
				<div>
					<label
						class="block text-xs text-accent uppercase tracking-wider mb-1.5"
					>
						Service Name <span class="text-destructive">*</span>
					</label>
					<input
						bind:value={form.name}
						maxlength="100"
						placeholder="e.g. General Consultation"
						class="w-full bg-accent/25 border rounded-lg px-3 py-2.5 text-sm text-primary-foreground
                   placeholder:text-background/70! outline-none transition-colors
                   focus:border-primary
                   {errors.name ? 'border-destructive' : 'border-accent/50'}"
					/>
					{#if errors.name}
						<p class="text-destructive text-xs mt-1">
							{errors.name}
						</p>
					{/if}
					<p class="text-background/50 text-xs mt-1">
						{form.name.length}/100
					</p>
				</div>

				<!-- Description -->
				<div>
					<label
						class="block text-xs text-accent uppercase tracking-wider mb-1.5"
					>
						Description <span class="text-destructive">*</span>
					</label>
					<textarea
						bind:value={form.description}
						rows="3"
						placeholder="Briefly describe this service..."
						class="w-full bg-accent/25 border rounded-lg px-3 py-2.5 text-sm text-primary-foreground
                   placeholder:text-background/70 outline-none resize-none transition-colors
                   focus:border-primary
                   {errors.description
							? 'border-destructive'
							: 'border-accent/50'}"
					></textarea>
					{#if errors.description}
						<p class="text-destructive text-xs mt-1">
							{errors.description}
						</p>
					{/if}
				</div>

				<!-- Duration + Priority -->
				<div class="grid grid-cols-2 gap-4">
					<div>
						<label
							class="block text-xs text-accent uppercase tracking-wider mb-1.5"
						>
							Duration (min) <span class="text-destructive"
								>*</span
							>
						</label>
						<input
							type="number"
							bind:value={form.duration}
							min="1"
							placeholder="15"
							class="w-full bg-accent/25 border rounded-lg px-3 py-2.5 text-sm text-primary-foreground
                     placeholder:text-accent outline-none transition-colors
                     focus:border-primary
                     {errors.duration
								? 'border-destructive'
								: 'border-accent/50'}"
						/>
						{#if errors.duration}
							<p class="text-destructive text-xs mt-1">
								{errors.duration}
							</p>
						{/if}
					</div>

					<div>
						<label
							class="block text-xs text-accent uppercase tracking-wider mb-1.5"
						>
							Priority Level
						</label>
						<select
							bind:value={form.priority}
							class="w-full bg-accent/25 border border-accent/50 rounded-lg px-3 py-2.5
                     text-sm text-primary-foreground outline-none focus:border-primary transition-colors"
						>
							<option value="low">Low</option>
							<option value="medium">Medium</option>
							<option value="high">High</option>
						</select>
					</div>
				</div>
			</div>

			<div
				class="px-6 py-4 border-t border-primary/20 flex justify-end gap-3"
			>
				<button
					on:click={() => (showForm = false)}
					class="px-4 py-2 text-sm rounded-lg cursor-pointer border border-accent/50 text-background
                 hover:bg-accent/25 transition-colors"
				>
					Cancel
				</button>
				<button
					on:click={save}
					disabled={saving}
					class="px-4 py-2 text-sm rounded-lg cursor-pointer bg-primary hover:bg-primary/80
                 text-foreground font-semibold transition-colors disabled:opacity-50"
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
