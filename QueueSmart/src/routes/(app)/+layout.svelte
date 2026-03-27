<script lang="ts">
	import { page } from "$app/state";
	import clsx from "clsx";
	import {
		LayoutDashboardIcon,
		BellIcon,
		UserIcon,
		LogOutIcon,
		MenuIcon,
	} from "@lucide/svelte";

	import { notificationStore } from "$lib/stores/notifications.svelte";

	let { children } = $props();

	let sidebarOpen = $state(false);

	const NAV_ITEMS = [
		{ label: "Queues", href: "/app", icon: LayoutDashboardIcon },
		{ label: "Notifications", href: "/app/notifications", icon: BellIcon },
		{ label: "Profile", href: "/app/profile", icon: UserIcon },
	];
</script>

<div class="flex h-dvh w-full bg-background text-foreground">
	<!-- Sidebar -->
	<aside
		class={clsx(
			"fixed inset-y-0 left-0 z-50 w-60 flex-shrink-0 flex flex-col bg-muted border-r border-primary/20 transition-transform duration-300 ease-in-out md:relative md:translate-x-0",
			!sidebarOpen && "-translate-x-full",
		)}
	>
		<div class="px-6 py-6 border-b border-primary/20">
			<span class="text-sm uppercase tracking-widest text-background"
				>QueueSmart</span
			>
			<h1 class="text-xl font-bold text-primary mt-1">Dashboard</h1>
		</div>

		<nav class="flex-1 py-6 px-3 flex flex-col gap-1">
			{#each NAV_ITEMS as item}
				{@const active = page.url.pathname === item.href}
				<a
					href={item.href}
					class={clsx(
						"flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm border transition-colors",
						active
							? "bg-primary/10 text-primary border-primary/20"
							: "text-background border-transparent hover:bg-primary/5",
					)}
				>
					<item.icon size={18} />
					{item.label}
					{#if item.label === "Notifications" && notificationStore.unreadCount > 0}
						<span
							class="ml-auto inline-flex size-4.5 font-sans items-center justify-center text-xs/none font-bold text-destructive-foreground rounded-full bg-destructive/70 border-destructive border"
						>
							{notificationStore.unreadCount}
						</span>
					{/if}
				</a>
			{/each}
		</nav>

		<div class="px-3 py-5 border-t border-primary/20">
			<a
				href="/"
				onclick={() => console.log("logout")}
				class="flex items-center gap-3 w-full px-4 py-2.5 rounded-lg text-sm text-background border border-transparent hover:bg-primary/5 transition-colors cursor-pointer"
			>
				<LogOutIcon size={18} />
				Logout
			</a>
		</div>
	</aside>

	<!-- Main area -->
	<div class="flex flex-1 flex-col overflow-hidden">
		<!-- Mobile header -->
		<header
			class="flex h-14 items-center border-b border-primary/20 bg-muted px-6 md:hidden"
		>
			<button
				class="mr-4 text-background cursor-pointer"
				onclick={() => (sidebarOpen = !sidebarOpen)}
			>
				<MenuIcon size={18} />
			</button>
			<span class="text-sm uppercase tracking-widest text-background"
				>Queue</span
			>
			<span class="text-sm uppercase tracking-widest text-primary"
				>Smart</span
			>
		</header>

		<main class="flex-1 overflow-y-auto">
			<div class="mx-auto max-w-6xl h-full py-6 md:py-8 px-6 md:px-8">
				{@render children()}
			</div>
		</main>
	</div>

	<!-- Mobile backdrop -->
	{#if sidebarOpen}
		<div
			class="fixed inset-0 z-40 bg-foreground/80 backdrop-blur-sm md:hidden"
			onclick={() => (sidebarOpen = false)}
			role="presentation"
		></div>
	{/if}
</div>
