<script lang="ts">
	import { goto } from "$app/navigation";
	import { page } from "$app/stores";
	import {
		ClockIcon,
		CogIcon,
		ListIcon,
		LogOutIcon,
		ShieldBan,
	} from "@lucide/svelte";
	import { onMount } from "svelte";
	import { get } from "svelte/store";

	import { authSession, clearAuthSession } from "$lib/stores/auth";

	let { children } = $props();

	const nav = [
		{ href: "/admin", label: "Dashboard", icon: ShieldBan },
		{ href: "/admin/services", label: "Service Management", icon: CogIcon },
		{ href: "/admin/queue", label: "Queue Management", icon: ListIcon },
		{ href: "/admin/history", label: "History", icon: ClockIcon },
	];

	let authReady = $state(false);

	onMount(() => {
		const session = get(authSession);

		if (!session) {
			goto("/login");
			return;
		}

		if (session.role === "User") {
			goto("/app");
			return;
		}

		authReady = true;
	});

	function handleLogout(event: MouseEvent): void {
		event.preventDefault();
		clearAuthSession();
		goto("/login");
	}
</script>

{#if authReady}
	<div
		class="flex h-screen font-sans overflow-hidden bg-foreground text-primary-foreground"
	>
		<!-- Sidebar -->
		<aside
			class="w-60 flex-shrink-0 flex flex-col bg-muted border-r border-primary/20"
		>
			<div class="px-6 py-6 border-b border-primary/20">
				<span class="text-sm uppercase tracking-widest text-background"
					>QueueSmart</span
				>
				<h1 class="text-xl font-bold text-primary mt-1">Admin Panel</h1>
			</div>

			<nav class="flex-1 py-6 px-3 flex flex-col gap-1">
				{#each nav as item}
					{@const active = $page.url.pathname === item.href}
					<a
						href={item.href}
						class={`flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm border transition-colors ${
							active
								? "bg-primary/10 text-primary border-primary/20"
								: "text-background border-transparent hover:bg-primary/5"
						}`}
					>
						<span class="text-base"><item.icon class="size-4" /></span>
						{item.label}
					</a>
				{/each}
			</nav>

			<div
				class="px-6 py-5 text-xs border-t border-primary/20 text-background"
			>
				<i class="opacity-75">THIS IS FAKE LOGIN INFO</i> Logged in as
				<span class="text-primary">admin@queuesmart.io</span>
			</div>

			<div class="px-3 py-5 border-t border-primary/20">
				<a
					href="/login"
					onclick={handleLogout}
					class="flex items-center gap-3 w-full px-4 py-2.5 rounded-lg text-sm text-background border border-transparent hover:bg-primary/5 transition-colors cursor-pointer"
				>
					<LogOutIcon size={18} />
					Logout
				</a>
			</div>
		</aside>

		<!-- Main content -->
		<main class="flex-1 overflow-y-auto">
			{@render children()}
		</main>
	</div>
{:else}
	<div class="flex h-screen items-center justify-center bg-foreground text-primary-foreground">
		<div class="text-center">
			<p class="text-sm uppercase tracking-[0.3em] text-primary/80">QueueSmart</p>
			<p class="mt-3 text-lg">Checking admin access...</p>
		</div>
	</div>
{/if}
