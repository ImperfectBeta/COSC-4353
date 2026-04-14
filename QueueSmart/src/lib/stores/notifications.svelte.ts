import { browser } from "$app/environment";

export interface Notification {
    id: string;
    title: string;
    message: string;
    timestamp: string;
    read: boolean;
    type: "queue_update" | "system" | "info";
    queueId?: string; 
}

class NotificationStore {
    notifications = $state<Notification[]>([]);
    unreadCount = $derived(this.notifications.filter(n => !n.read).length);

    constructor() {
        this.notifications = [];
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