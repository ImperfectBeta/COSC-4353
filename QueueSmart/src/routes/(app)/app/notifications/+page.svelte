<script lang="ts">
	import { notificationStore } from "$lib/stores/notifications.svelte";
	import { BellIcon, InfoIcon, CircleAlertIcon } from "@lucide/svelte";
	import Button from "$lib/components/button.svelte";

	import { onMount } from "svelte";

	onMount(() => {
		notificationStore.markAllAsRead();
	});

	let queueUpdates = $derived(
		notificationStore.notifications.filter(n => n.type === "queue_update"),
	);

	let otherNotifications = $derived(
		notificationStore.notifications.filter(n => n.type !== "queue_update"),
	);

	function formatDate(dateString: string) {
		const date = new Date(dateString);
		return new Intl.DateTimeFormat("en-US", {
			month: "short",
			day: "numeric",
			hour: "numeric",
			minute: "numeric",
		}).format(date);
	}
</script>

<div class="space-y-8">
	<section>
		<h1 class="text-3xl font-bold tracking-tight">Notifications</h1>
		<p class="text-foreground/70 mt-2">
			This is some example text. What's good broh? How's it goin broh? I
			hope you're havin a good one broh.
		</p>
	</section>

	{#if notificationStore.notifications.length === 0}
		<div
			class="rounded-lg border border-dashed p-12 text-center text-foreground/70 flex flex-col items-center justify-center gap-4"
		>
			<BellIcon class="size-12 opacity-20" />
			<p>You have no notifications at this time.</p>
		</div>
	{:else}
		{#if queueUpdates.length > 0}
			<section class="space-y-4">
				<h2
					class="text-xl font-semibold tracking-tight flex items-center gap-2"
				>
					<CircleAlertIcon class="size-5 text-muted" />
					Updates to Your Active Queues
				</h2>
				<div class="grid gap-4">
					{#each queueUpdates as notification (notification.id)}
						<div
							class="flex flex-col gap-2 rounded-lg border bg-card p-4 transition-colors shadow-md shadow-accent/50 hover:bg-muted/90 bg-muted"
						>
							<div class="flex items-start justify-between gap-4">
								<div class="space-y-1">
									<h3
										class="font-semibold text-lg leading-none text-muted-foreground tracking-tight"
									>
										{notification.title}
									</h3>
									<p class="text-sm text-muted-foreground/80">
										{notification.message}
									</p>
								</div>
								<span
									class="text-sm text-muted-foreground whitespace-nowrap"
								>
									{formatDate(notification.timestamp)}
								</span>
							</div>
							{#if notification.queueId}
								<div class="mt-2 w-fit">
									<a
										href={`/app/queue/${notification.queueId}`}
									>
										<Button variant="outline" size="sm">
											View Queue
										</Button>
									</a>
								</div>
							{/if}
						</div>
					{/each}
				</div>
			</section>
		{/if}

		{#if otherNotifications.length > 0}
			<section class="space-y-4">
				<h2
					class="text-xl font-semibold tracking-tight flex text-accent items-center gap-2"
				>
					<InfoIcon class="size-5 text-accent" />
					Other Updates
				</h2>
				<div class="grid gap-4">
					{#each otherNotifications as notification (notification.id)}
						<div
							class="flex flex-col gap-1 rounded-lg border shadow-md shadow-accent/50 bg-accent p-4 transition-colors hover:bg-accent/90"
						>
							<div class="flex items-start justify-between gap-4">
								<div class="space-y-1">
									<h3
										class="font-medium leading-none text-lg text-accent-foreground"
									>
										{notification.title}
									</h3>
									<p
										class="text-sm text-accent-foreground/80"
									>
										{notification.message}
									</p>
								</div>
								<span
									class="text-sm text-muted-foreground whitespace-nowrap"
								>
									{formatDate(notification.timestamp)}
								</span>
							</div>
						</div>
					{/each}
				</div>
			</section>
		{/if}
	{/if}
</div>
