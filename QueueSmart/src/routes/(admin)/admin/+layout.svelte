<script lang="ts">
    import { goto } from "$app/navigation";
    import { page } from "$app/stores";
    import {
        ClockIcon,
        CogIcon,
        ListIcon,
        LogOutIcon,
        ShieldBan,
        CircleUser,
        ChartColumn
    } from "@lucide/svelte";
    import { onMount } from "svelte";
    import { get } from "svelte/store";
    import clsx from "clsx";

    import { authSession, clearAuthSession } from "$lib/stores/auth";

    let { children } = $props();

    const nav = [
        { href: "/admin", label: "Dashboard", icon: ShieldBan },
        { href: "/admin/services", label: "Service Management", icon: CogIcon },
        { href: "/admin/queue", label: "Queue Management", icon: ListIcon },
        { href: "/admin/history", label: "History", icon: ClockIcon },
        { href: "/admin/reports", label: "Reports", icon: ChartColumn },
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
    <div class="flex h-screen font-['Inter'] overflow-hidden bg-[#0B132B] text-white">
        <!-- Sidebar -->
        <aside class="w-64 flex-shrink-0 flex flex-col bg-[#1C2541] border-r border-white/10 shadow-xl">
            
            <div class="px-6 py-8 border-b border-white/10">
                <a href="/">
                    <h1 class="text-2xl font-bold font-['Righteous'] tracking-wide">
                        Queue<span class="text-[#5BC0BE]">Smart</span>
                    </h1>
                </a>
                <p class="text-xs uppercase tracking-widest text-[#5BC0BE] mt-1 font-semibold">Admin Panel</p>
            </div>

            <nav class="flex-1 py-6 px-4 flex flex-col gap-2">
                {#each nav as item}
                    {@const active = $page.url.pathname === item.href}
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
                    </a>
                {/each}
            </nav>

            <div class="px-4 py-5 border-t border-white/10 flex items-center gap-3 text-sm text-white/80">
                <div class="bg-[#5BC0BE]/20 p-2 rounded-full">
                    <CircleUser size={20} class="text-[#5BC0BE]" />
                </div>
                <div class="flex flex-col overflow-hidden">
                    <span class="font-semibold truncate">{$authSession?.email?.split('@')[0] || "Admin"}</span>
                    <span class="text-xs text-white/50 truncate">{$authSession?.email || "admin@queuesmart.io"}</span>
                </div>
            </div>

            <div class="px-4 py-4 mb-4">
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

        <!-- Main content -->
        <main class="flex-1 overflow-y-auto">
            {@render children()}
        </main>
    </div>
{:else}
    <div class="flex h-screen items-center justify-center bg-[#3A506B] text-white">
        <div class="text-center">
            <h1 class="text-3xl font-bold font-['Righteous'] tracking-wide mb-4 animate-pulse">
                Queue<span class="text-[#5BC0BE]">Smart</span>
            </h1>
            <p class="text-lg text-white/80">Checking admin access...</p>
        </div>
    </div>
{/if}