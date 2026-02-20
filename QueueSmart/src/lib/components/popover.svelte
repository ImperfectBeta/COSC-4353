<script lang="ts">
	import clsx from "clsx";
	import type { Snippet } from "svelte";
	import { on } from "svelte/events";

	let {
		body,
		trigger,
		position,
		align,
		distanceFromTrigger,
		className,
		closeOnClickOutside = true,
	}: {
		body: Snippet;
		trigger: Snippet<
			[{ open: boolean; setOpen: (state: boolean) => void }]
		>;
		position?: "top" | "bottom" | "left" | "right";
		align?: "start" | "end";
		distanceFromTrigger?: number;
		className?: string;
		closeOnClickOutside?: boolean;
	} = $props();

	let open = $state(false);

	function setOpen(state: boolean) {
		open = state;
	}

	on(document, "click", e => {
		const target = e.target as Element | null;
		if (!target || target.closest(".popover") || !closeOnClickOutside)
			return;
		setOpen(false);
	});
</script>

<div class="relative popover">
	{@render trigger({ open, setOpen })}
	{#if open}
		<div
			style="--distance: {distanceFromTrigger ?? 8}px;"
			data-position={position ?? "bottom"}
			role="dialog"
			aria-modal={closeOnClickOutside}
			class={clsx(
				"popover-body absolute p-3 min-w-48 bg-background border border-muted/25 shadow-md rounded-md z-50",
				position === "top" && "bottom-full left-1/2 -translate-x-1/2",
				position === "bottom" && "top-full left-1/2 -translate-x-1/2",
				position === "left" && "right-full top-1/2 -translate-y-1/2",
				position === "right" && "left-full top-1/2 -translate-y-1/2",
				!position && "top-full left-0",
				align === "start" &&
					(position === "top" || position === "bottom") &&
					"left-0 translate-x-0",
				align === "end" &&
					(position === "top" || position === "bottom") &&
					"right-0 left-auto translate-x-0",
				className,
			)}
		>
			{@render body()}
		</div>
	{/if}
</div>

<style>
	.popover-body[data-position="top"] {
		margin-bottom: var(--distance);
	}
	.popover-body[data-position="bottom"] {
		margin-top: var(--distance);
	}
	.popover-body[data-position="left"] {
		margin-right: var(--distance);
	}
	.popover-body[data-position="right"] {
		margin-left: var(--distance);
	}
</style>
