<script lang="ts">
	import Menu from "@lucide/svelte/icons/menu";
	let { children } = $props();
</script>

<!-- Here we would probably hide /app and /login if the user is logged in, and show a logout button instead -->

<header>
	<nav
		class="flex justify-between items-center py-3 px-6 bg-accent font-nunito font-medium tracking-tight text-base text-accent-foreground"
	>
		<div class="flex items-center gap-3">
			<Menu />
			<a href="/"><h1 class="text-2xl font-bold">QueueSmart</h1></a>
		</div>
		<ul class="flex gap-8">
			<li><a href="/">Home</a></li>
			<li><a href="/login">Login</a></li>
			<li><a href="/app">App</a></li>
			<li><a href="/admin">Admin</a></li>
		</ul>
	</nav>
</header>

{@render children()}
