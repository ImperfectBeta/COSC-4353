<script lang="ts">
    import { goto } from "$app/navigation";
    import { page } from "$app/state";
    import {
        BellIcon,
        LayoutDashboardIcon,
        LogOutIcon,
        MenuIcon,
        UserIcon,
    } from "@lucide/svelte";
    import clsx from "clsx";
    import { onMount } from "svelte";
    import { get } from "svelte/store";

    import { authSession, clearAuthSession } from "$lib/stores/auth";
    import { notificationStore } from "$lib/stores/notifications.svelte";

    let { children } = $props();

    let sidebarOpen = $state(false);
    let authReady = $state(false);

    const NAV_ITEMS = [
        { label: "Queues", href: "/app", icon: LayoutDashboardIcon },
        { label: "Notifications", href: "/app/notifications", icon: BellIcon },
        { label: "Profile", href: "/app/profile", icon: UserIcon },
    ];

    onMount(() => {
        const session = get(authSession);

        if (!session) {
            goto("/login");
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
    <div class="flex h-dvh w-full bg-[#F4F4F4] text-[#0B132B] font-['Inter']">
        <aside
            class={clsx(
                "fixed inset-y-0 left-0 z-50 w-64 flex-shrink-0 flex flex-col bg-[#1C2541] text-white transition-transform duration-300 ease-in-out md:relative md:translate-x-0 shadow-xl",
                !sidebarOpen && "-translate-x-full",
            )}
        >
            <div class="px-6 py-8 border-b border-white/10">
                <a href="/">
                    <h1 class="text-2xl font-bold font-['Righteous'] tracking-wide">
                        Queue<span class="text-[#5BC0BE]">Smart</span>
                    </h1>
                </a>
            </div>

            <nav class="flex-1 py-6 px-4 flex flex-col gap-2">
                {#each NAV_ITEMS as item}
                    {@const active = page.url.pathname === item.href}
                    <a
                        href={item.href}
                        class={clsx(
                            "flex items-center gap-3 px-4 py-3 rounded-[8px] text-[15px] font-medium transition-all duration-200",
                            active
                                ? "bg-[#5BC0BE] text-[#1C2541] shadow-md"
                                : "text-white/70 hover:bg-white/10 hover:text-white",
                        )}
                    >
                        <item.icon size={20} />
                        {item.label}
                        {#if item.label === "Notifications" && notificationStore.unreadCount > 0}
                            <span
                                class="ml-auto inline-flex h-5 w-5 items-center justify-center text-[11px] font-bold text-white rounded-full bg-red-500 shadow-sm"
                            >
                                {notificationStore.unreadCount}
                            </span>
                        {/if}
                    </a>
                {/each}
            </nav>

            <div class="px-4 py-6 border-t border-white/10">
                <a
                    href="/login"
                    onclick={handleLogout}
                    class="flex items-center gap-3 w-full px-4 py-3 rounded-[8px] text-[15px] font-medium text-white/70 hover:bg-red-500/10 hover:text-red-400 transition-colors cursor-pointer"
                >
                    <LogOutIcon size={20} />
                    Logout
                </a>
            </div>
        </aside>

        <div class="flex flex-1 flex-col overflow-hidden">
            <header
                class="flex h-16 items-center justify-between border-b border-gray-200 bg-white px-6 md:hidden shadow-sm"
            >
                <div class="flex items-center">
                    <button
                        class="mr-4 text-[#1C2541] cursor-pointer"
                        onclick={() => (sidebarOpen = !sidebarOpen)}
                    >
                        <MenuIcon size={24} />
                    </button>
                    <h1 class="text-xl font-bold font-['Righteous'] tracking-wide">
                        Queue<span class="text-[#5BC0BE]">Smart</span>
                    </h1>
                </div>
            </header>

            <main class="flex-1 overflow-y-auto">
                <div class="mx-auto max-w-6xl h-full py-8 px-6 md:px-10">
                    {@render children()}
                </div>
            </main>
        </div>

        {#if sidebarOpen}
            <div
                class="fixed inset-0 z-40 bg-[#0B132B]/60 backdrop-blur-sm md:hidden"
                onclick={() => (sidebarOpen = false)}
                role="presentation"
            ></div>
        {/if}
    </div>
{:else}
    <div class="flex h-dvh w-full items-center justify-center bg-[#3A506B] text-white">
        <div class="text-center">
            <h1 class="text-3xl font-bold font-['Righteous'] tracking-wide mb-4 animate-pulse">
                Queue<span class="text-[#5BC0BE]">Smart</span>
            </h1>
            <p class="text-lg text-white/80">Checking your session...</p>
        </div>
    </div>
{/if}