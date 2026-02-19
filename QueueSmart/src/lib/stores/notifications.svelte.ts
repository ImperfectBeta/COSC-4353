import { browser } from "$app/environment";

export interface Notification {
	id: string;
	title: string;
	message: string;
	timestamp: string;
	read: boolean;
	type: "queue_update" | "system" | "info";
	queueId?: string; // for optionally linking to a specific queue
}

class NotificationStore {
	notifications = $state<Notification[]>([]);

	unreadCount = $derived(this.notifications.filter(n => !n.read).length);

	constructor() {
		// mock data for demonstration
		this.notifications = [
			{
				id: "1",
				title: "Your turn is coming up!",
				message: "You are next in line for the Financial Aid queue.",
				timestamp: new Date().toISOString(),
				read: false,
				type: "queue_update",
				queueId: "q123",
			},
			{
				id: "2",
				title: "Welcome to QueueSmart",
				message: "Thanks for joining! Usage tips here.",
				timestamp: new Date(Date.now() - 86400000).toISOString(), // 1 day ago
				read: true,
				type: "system",
			},
			{
				id: "3",
				title: "Queue Paused",
				message: "The Registrar queue has been temporarily paused.",
				timestamp: new Date(Date.now() - 3600000).toISOString(), // 1 hour ago
				read: false,
				type: "queue_update",
				queueId: "q124",
			},
		];
	}

	addNotification(
		notification: Omit<Notification, "id" | "timestamp" | "read">,
	) {
		const newNotification: Notification = {
			...notification,
			id: crypto.randomUUID(),
			timestamp: new Date().toISOString(),
			read: false,
		};
		this.notifications = [newNotification, ...this.notifications];
	}

	markAllAsRead() {
		this.notifications = this.notifications.map(n => ({
			...n,
			read: true,
		}));
	}

	markAsRead(id: string) {
		const index = this.notifications.findIndex(n => n.id === id);
		if (index !== -1) {
			this.notifications[index].read = true;
		}
	}
}

export const notificationStore = new NotificationStore();
