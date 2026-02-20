<script lang="ts">
	import { cn } from "$lib/utils";
	import type { Snippet } from "svelte";
	import type { HTMLButtonAttributes } from "svelte/elements";

	type Size = "sm" | "md" | "lg";
	type Variant =
		| "primary"
		| "outline"
		| "ghost"
		| "ghostWhite"
		| "accent"
		| "destructive";

	const STYLE_MAP: Record<"sizes", Record<Size, string>> &
		Record<"variants", Record<Variant, string>> = {
		sizes: {
			sm: "px-3 py-0.5 gap-1.5 text-sm",
			md: "px-3 py-1 gap-2 text-base",
			lg: "px-4 py-2 gap-2 text-base",
		},
		variants: {
			primary: "bg-primary hover:bg-primary/80 text-primary-foreground",
			outline:
				"border border-primary hover:bg-primary hover:border-transparent bg-transparent text-primary hover:text-primary-foreground",
			ghost: "bg-transparent hover:bg-primary text-foreground hover:text-primary-foreground",
			ghostWhite:
				"bg-transparent hover:bg-white text-white hover:text-foreground",
			accent: "bg-accent hover:bg-accent/80 text-accent-foreground",
			destructive:
				"bg-destructive hover:bg-destructive/80 text-destructive-foreground",
		},
	};

	type Props = {
		children: Snippet;
		size?: Size;
		variant?: Variant;
	} & HTMLButtonAttributes;

	let {
		children,
		size = "md",
		variant = "primary",
		class: className,
		...rest
	}: Props = $props();
</script>

<button
	class={cn(
		"p-3 flex items-center disabled:opacity-50 disabled:pointer-events-none rounded-full cursor-pointer font-sans font-semibold",
		STYLE_MAP.sizes[size],
		STYLE_MAP.variants[variant],
		className,
	)}
	{...rest}
>
	{@render children()}
</button>
