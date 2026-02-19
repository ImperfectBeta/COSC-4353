import { writable } from 'svelte/store';

export const services = writable([
    {
        id: 1,
        name: 'General Consultation',
        description: 'Walk-in general medical consultation.',
        duration: 15,
        priority: 'high',
        isOpen: true,
        queueLength: 8,
    },
    {
        id: 2,
        name: 'Document Processing',
        description: 'Submission and processing of official documents.',
        duration: 10,
        priority: 'medium',
        isOpen: true,
        queueLength: 3,
    },
    {
        id: 3,
        name: 'Technical Support',
        descrition: 'IT and device troubleshooting desk',
        duration: 20,
        priority: 'low',
        isOpen: false,
        queueLength: 0,
    },
]);

export const queues = writable({
    1: [
        { id: 101, name: 'Alice Johnson', joinedAt: '09:02 AM', status: 'waiting' },
        { id: 102, name: 'Bob Smith',     joinedAt: '09:10 AM', status: 'waiting' },
        { id: 103, name: 'Carol White',   joinedAt: '09:18 AM', status: 'waiting' },
    ],
    2: [
        { id: 201, name: 'Dan Brown',     joinedAt: '09:05 AM', status: 'waiting' },
        { id: 202, name: 'Eve Davis',     joinedAt: '09:22 AM', status: 'waiting' },
    ],
    3: [],
});
