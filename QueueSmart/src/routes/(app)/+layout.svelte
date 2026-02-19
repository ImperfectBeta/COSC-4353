<script lang="ts">
	import { page } from "$app/state";
	import Button from "$lib/components/button.svelte";
	import clsx from "clsx";
	import {
		ListCheckIcon,
		BellIcon,
		UserIcon,
		LayoutDashboardIcon,
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

<div class="flex h-dvh w-full bg-primary/10">
	<aside
		class={clsx(
			"fixed inset-y-0 left-0 z-50 w-64 bg-accent text-accent-foreground transition-transform duration-300 ease-in-out md:relative md:translate-x-0",
			!sidebarOpen && "-translate-x-full",
		)}
	>
		<div class="flex h-full flex-col">
			<div class="flex h-16 items-center gap-0.5 px-6">
				<ListCheckIcon class="size-7 text-primary" />
				<h1 class="text-2xl font-arvo font-black tracking-tight">
					<span class="text-primary">Queue</span>Smart
				</h1>
			</div>

			<nav class="flex-1 space-y-1 p-4">
				{#each NAV_ITEMS as item}
					<a
						href={item.href}
						class={clsx(
							"flex items-center gap-3 rounded-md px-3 py-2 text-base font-medium hover:bg-muted hover:text-muted-foreground",
							page.url.pathname === item.href
								? "bg-muted text-muted-foreground"
								: "text-accent-foreground/80",
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

			<div class="px-4 py-4">
				<Button
					class="w-full text-accent-foreground text-base rounded-md py-1.5"
					variant="ghostWhite"
					size="sm"
					onclick={() => console.log("logout")}
				>
					<LogOutIcon size={18} />
					<span>Logout</span>
				</Button>
			</div>
		</div>
	</aside>

	<div class="flex flex-1 flex-col overflow-hidden">
		<header
			class="flex h-16 items-center border-b border-accent/35 bg-background px-6 md:hidden"
		>
			<button
				class="mr-4 text-foreground md:hidden"
				onclick={() => (sidebarOpen = !sidebarOpen)}
			>
				<MenuIcon size={18} />
			</button>
			<h1 class="text-lg font-semibold">QueueSmart</h1>
		</header>

		<main class="flex-1 overflow-y-auto">
			<div class="mx-auto max-w-6xl h-full py-6 md:py-8 px-6 md:px-8">
				{@render children()}
			</div>
		</main>
	</div>

	{#if sidebarOpen}
		<div
			class="fixed inset-0 z-40 bg-background/80 backdrop-blur-sm md:hidden"
			onclick={() => (sidebarOpen = false)}
			role="presentation"
		></div>
	{/if}
</div>
