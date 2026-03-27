<script lang="ts">
	import { page } from "$app/stores";
	import {
		CogIcon,
		ListIcon,
		ShieldBan,
		ClockIcon,
		LogOutIcon,
	} from "@lucide/svelte";

	const nav = [
		{ href: "/admin", label: "Dashboard", icon: ShieldBan },
		{ href: "/admin/services", label: "Service Management", icon: CogIcon },
		{ href: "/admin/queue", label: "Queue Management", icon: ListIcon },
		{ href: "/admin/history", label: "History", icon: ClockIcon },
	];
</script>

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
					<span class="text-base"
						><svelte:component
							this={item.icon}
							class="size-4"
						/></span
					>
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
				href="/"
				onclick={() => console.log("logout")}
				class="flex items-center gap-3 w-full px-4 py-2.5 rounded-lg text-sm text-background border border-transparent hover:bg-primary/5 transition-colors cursor-pointer"
			>
				<LogOutIcon size={18} />
				Logout
			</a>
		</div>
	</aside>

	<!-- Main content -->
	<main class="flex-1 overflow-y-auto">
		<slot />
	</main>
</div>
