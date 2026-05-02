<script lang="ts">
	import Sparkles from "@lucide/svelte/icons/sparkles";
	import X from "@lucide/svelte/icons/x";
	import Send from "@lucide/svelte/icons/send";
	import Bot from "@lucide/svelte/icons/bot";
	import type { Queue } from "$lib/types";

	interface Props {
		queues: Queue[];
		userId: number;
	}

	let { queues, userId }: Props = $props();

	let isOpen = $state(false);
	let input = $state("");
	let loading = $state(false);
	let messages = $state<Array<{ role: "user" | "assistant"; text: string }>>(
		[],
	);
	let messagesEl: HTMLElement | undefined = $state();

	// Generate dynamic suggested questions from live queue data
	let suggestions = $derived.by(() => {
		const open = queues.filter(q => q.status === "open");
		const items: string[] = [];

		if (open.length > 0) {
			items.push(`How long is the wait for ${open[0].name}?`);
		}
		items.push("Which queue has the shortest wait?");
		items.push("What's my current position in line?");
		if (open.length > 1) {
			items.push(`Should I join ${open[open.length - 1].name}?`);
		}

		return items.slice(0, 3);
	});

	let showSuggestions = $derived(messages.length === 0);

	function scrollToBottom() {
		setTimeout(() => {
			if (messagesEl) messagesEl.scrollTop = messagesEl.scrollHeight;
		}, 50);
	}

	async function sendMessage(text?: string) {
		const msg = text || input.trim();
		if (!msg || loading) return;

		input = "";
		messages = [...messages, { role: "user", text: msg }];
		loading = true;
		scrollToBottom();

		try {
			const res = await fetch("/assistant", {
				method: "POST",
				headers: { "Content-Type": "application/json" },
				body: JSON.stringify({
					message: msg,
					userId,
					history: messages.slice(0, -1), // previous messages (exclude current)
				}),
			});

			const data = await res.json();
			messages = [...messages, { role: "assistant", text: data.reply }];
		} catch {
			messages = [
				...messages,
				{
					role: "assistant",
					text: "Sorry, something went wrong. Please try again.",
				},
			];
		} finally {
			loading = false;
			scrollToBottom();
		}
	}

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === "Enter" && !e.shiftKey) {
			e.preventDefault();
			sendMessage();
		}
	}

	function toggle() {
		isOpen = !isOpen;
	}
</script>

<!-- Floating Action Button -->
{#if !isOpen}
	<button
		onclick={toggle}
		class="fixed bottom-6 right-6 z-50 flex h-14 w-14 items-center justify-center rounded-full bg-gradient-to-br from-[#5BC0BE] to-[#3A9E9C] text-white shadow-lg shadow-[#5BC0BE]/30 transition-all duration-300 hover:scale-110 hover:shadow-xl hover:shadow-[#5BC0BE]/40 active:scale-95 cursor-pointer"
		aria-label="Open AI Assistant"
	>
		<Sparkles size={24} />
	</button>
{/if}

<!-- Chat Panel -->
{#if isOpen}
	<div
		class="fixed bottom-6 right-6 z-50 flex w-[380px] flex-col overflow-hidden rounded-2xl border border-white/10 bg-[#1C2541] shadow-2xl shadow-black/40 animate-in"
		style="height: min(520px, calc(100dvh - 3rem));"
	>
		<!-- Header -->
		<div
			class="flex items-center justify-between border-b border-white/10 bg-[#0B132B] px-5 py-4"
		>
			<div class="flex items-center gap-3">
				<div>
					<h3 class="text-sm font-bold text-white">QueueSmart AI</h3>
					<p class="text-[11px] text-white/50">
						Powered by Chase's $10 in Gemini credits
					</p>
				</div>
			</div>
			<button
				onclick={toggle}
				class="rounded-lg p-1.5 text-white/50 transition-colors hover:bg-white/10 hover:text-white cursor-pointer"
				aria-label="Close chat"
			>
				<X size={18} />
			</button>
		</div>

		<!-- Messages -->
		<div
			bind:this={messagesEl}
			class="flex flex-1 flex-col gap-3 overflow-y-auto px-4 py-4 scroll-smooth"
		>
			{#if messages.length === 0 && !loading}
				<!-- Welcome state -->
				<div
					class="flex flex-1 flex-col items-center justify-center gap-4 text-center"
				>
					<div
						class="flex h-12 w-12 items-center justify-center rounded-2xl bg-[#5BC0BE]/15"
					>
						<Bot size={24} class="text-[#5BC0BE]" />
					</div>
					<div>
						<p class="text-sm font-semibold text-white">
							Hi! I'm your queue assistant.
						</p>
						<p class="mt-1 text-xs text-white/50">
							Ask me about wait times, queue lengths, or services.
							I'll even give you a recipe for banana bread!
						</p>
					</div>
				</div>
			{/if}

			{#each messages as msg, i}
				<div
					class="flex {msg.role === 'user'
						? 'justify-end'
						: 'justify-start'}"
				>
					<div
						class="max-w-[85%] rounded-2xl px-4 py-2.5 text-sm leading-relaxed {msg.role ===
						'user'
							? 'bg-[#5BC0BE] text-[#0B132B] rounded-br-md font-medium'
							: 'bg-white/10 text-white/90 rounded-bl-md'}"
					>
						{msg.text}
					</div>
				</div>
			{/each}

			{#if loading}
				<div class="flex justify-start">
					<div
						class="flex items-center gap-1.5 rounded-2xl rounded-bl-md bg-white/10 px-4 py-3"
					>
						<span class="typing-dot"></span>
						<span class="typing-dot" style="animation-delay: 0.15s"
						></span>
						<span class="typing-dot" style="animation-delay: 0.3s"
						></span>
					</div>
				</div>
			{/if}
		</div>

		<!-- Suggested Questions -->
		{#if showSuggestions}
			<div class="flex flex-wrap gap-2 border-t border-white/5 px-4 py-3">
				{#each suggestions as suggestion}
					<button
						onclick={() => sendMessage(suggestion)}
						class="rounded-full border border-[#5BC0BE]/30 bg-[#5BC0BE]/10 px-3 py-1.5 text-xs font-medium text-[#5BC0BE] transition-all hover:bg-[#5BC0BE]/20 hover:border-[#5BC0BE]/50 cursor-pointer"
					>
						{suggestion}
					</button>
				{/each}
			</div>
		{/if}

		<!-- Input -->
		<div class="border-t border-white/10 bg-[#0B132B] px-4 py-3">
			<div class="flex items-center gap-2">
				<input
					type="text"
					bind:value={input}
					onkeydown={handleKeydown}
					placeholder="Ask about queues..."
					disabled={loading}
					class="flex-1 rounded-xl border border-white/10 bg-white/5 px-4 py-2.5 text-sm text-white placeholder:text-white/30 outline-none transition-colors focus:border-[#5BC0BE]/50 focus:bg-white/[0.07] disabled:opacity-50"
				/>
				<button
					onclick={() => sendMessage()}
					disabled={loading || !input.trim()}
					class="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-[#5BC0BE] text-[#0B132B] transition-all hover:bg-[#4aa8a6] disabled:opacity-30 disabled:cursor-not-allowed cursor-pointer"
					aria-label="Send message"
				>
					<Send size={16} />
				</button>
			</div>
		</div>
	</div>
{/if}

<style>
	.animate-in {
		animation: slideUp 0.25s ease-out;
	}

	@keyframes slideUp {
		from {
			opacity: 0;
			transform: translateY(12px) scale(0.97);
		}
		to {
			opacity: 1;
			transform: translateY(0) scale(1);
		}
	}

	.typing-dot {
		width: 6px;
		height: 6px;
		border-radius: 50%;
		background-color: rgba(91, 192, 190, 0.7);
		animation: typingPulse 1s ease-in-out infinite;
	}

	@keyframes typingPulse {
		0%,
		100% {
			opacity: 0.3;
			transform: scale(0.8);
		}
		50% {
			opacity: 1;
			transform: scale(1);
		}
	}
</style>
