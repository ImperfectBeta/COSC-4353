<script lang="ts">
	import clsx from "clsx";
	import type { Component } from "svelte";

	export type BadgeVariant =
		| "default"
		| "secondary"
		| "destructive"
		| "outline"
		| "success"
		| "warning";

	let {
		children,
		variant = "default",
		className,
		icon,
	}: {
		children: any;
		variant?: BadgeVariant;
		className?: string;
		icon?: Component;
	} = $props();

	const variants: Record<BadgeVariant, { class: string }> = {
		default: {
			class: "border-transparent bg-primary text-primary-foreground",
		},
		secondary: {
			class: "border-transparent bg-secondary text-secondary-foreground",
		},
		destructive: {
			class: "border-transparent bg-destructive text-destructive-foreground",
		},
		outline: { class: "text-foreground" },
		success: {
			class: "border-transparent bg-green-300 text-foreground",
		},
		warning: {
			class: "border-transparent bg-yellow-300 text-foreground",
		},
	};

	let Icon = $derived(icon);
</script>

<div
	class={clsx(
		"inline-flex items-center select-none rounded-md border px-2.5 py-0.5 text-xs uppercase font-sans font-bold focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2",
		variants[variant].class,
		className,
	)}
>
	{@render children()}
	{#if Icon}
		<Icon class="ml-1 size-3.5" />
	{/if}
</div>
