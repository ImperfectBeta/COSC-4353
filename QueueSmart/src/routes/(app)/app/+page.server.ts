import type { PageServerLoad } from "./$types";
import type { User, Queue, QueueEntry } from "$lib/types";

const MOCK_USER: User = {
	id: "u1",
	name: "Peter Griffin",
	email: "[EMAIL_ADDRESS]",
	role: "user",
};

const MOCK_QUEUES: Queue[] = [
	{
		id: "q1",
		name: "General Support",
		description: "Get help with general account issues and questions.",
		organization: "Customer Service",
		status: "open",
		priority: "medium",
		currentLength: 5,
		averageWaitTime: 10,
	},
	{
		id: "q2",
		name: "Emergency Room",
		description: "Urgent care for critical medical conditions.",
		organization: "City Hospital",
		status: "open",
		priority: "high",
		currentLength: 12,
		averageWaitTime: 45,
	},
	{
		id: "q3",
		name: "DMV Renewal",
		description: "License and registration renewals.",
		organization: "Department of Motor Vehicles",
		status: "paused",
		priority: "low",
		currentLength: 45,
		averageWaitTime: 120,
	},
	{
		id: "q4",
		name: "Academic Advising",
		description: "Spring semester course selection advice.",
		organization: "University College",
		status: "open",
		priority: "medium",
		currentLength: 3,
		averageWaitTime: 15,
	},
];

const MOCK_ACTIVE_ENTRIES: (QueueEntry & { queue: Queue })[] = [
	{
		id: "e1",
		queueId: "q1",
		userId: "u1",
		entryTime: new Date(Date.now() - 15 * 60000).toISOString(),
		position: 2,
		estimatedWaitTime: 12,
		status: "waiting",
		queue: MOCK_QUEUES[0],
	},
];

export const load: PageServerLoad = async () => {
	// we will fetch this data from a database
	return {
		user: MOCK_USER,
		activeQueues: MOCK_ACTIVE_ENTRIES,
		suggestedQueues: MOCK_QUEUES.filter(q => q.id !== "q1"), // exclude already joined
	};
};
