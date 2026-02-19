// USERS

export interface User {
	id: string;
	name: string;
	email: string;
	role: "user" | "admin";
}

// QUEUES

export type QueueStatus = "open" | "closed" | "paused";
export type QueuePriority = "low" | "medium" | "high";

export interface Queue {
	id: string;
	name: string;
	description: string;
	organization: string;
	status: QueueStatus;
	priority: QueuePriority;
	currentLength: number;
	averageWaitTime: number; // in minutes per person
}

// QUEUE ENTRIES

export type QueueEntryStatus = "waiting" | "called" | "serviced" | "cancelled";

export interface QueueEntry {
	id: string;
	queueId: string;
	userId: string;
	entryTime: string;
	position: number;
	estimatedWaitTime: number; // in minutes
	status: QueueEntryStatus;
}
