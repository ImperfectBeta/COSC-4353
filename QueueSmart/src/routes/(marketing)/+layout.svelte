<script lang="ts">
    import { authSession } from "$lib/stores/auth";
    
    let { children } = $props();
    let isLoggedIn = $state(false);
    
    authSession.subscribe(session => {
        isLoggedIn = !!session;
    });
</script>

<header>
    <nav class="flex justify-between items-center py-0 px-6 md:px-12 bg-[#1C2541] font-nunito font-medium text-base text-white shadow-md relative z-10 h-[60px]">
        
        <div class="flex items-center">
            <a href="/">
                <h1 class="text-xl md:text-2xl font-bold font-['Righteous'] tracking-wide">
                    Queue<span class="text-[#5BC0BE]">Smart</span>
                </h1>
            </a>
        </div>

        <ul class="flex items-center gap-6">
            {#if isLoggedIn}
                <li><a href="/app" class="hover:text-[#5BC0BE] transition-colors">App</a></li>
                <li><a href="/admin" class="hover:text-[#5BC0BE] transition-colors">Admin</a></li>
                <li>
                    <a href="/app/profile" class="text-white/70 hover:text-white transition-colors text-sm">Profile</a>
                </li>
            {:else}
                <li><a href="/login" class="hover:text-[#5BC0BE] transition-colors">Login</a></li>
                <li>
                    <a 
                        href="/register" 
                        class="px-5 py-2 bg-[#5BC0BE] hover:bg-[#4aa8a6] text-[#1C2541] rounded-[6px] font-bold text-[15px] transition-all duration-200 shadow-sm hover:shadow-md"
                    >
                        Get Started
                    </a>
                </li>
            {/if}
        </ul>
    </nav>
</header>

<main class="h-[calc(100vh-60px)]">
    {@render children()}
</main>