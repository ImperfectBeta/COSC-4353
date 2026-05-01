import type { PageServerLoad } from "./$types";
import type { Queue, QueueEntry } from "$lib/types";
import { env } from '$env/dynamic/public';

export const load: PageServerLoad = async ({ cookies, fetch }) => {
    const sessionStr = cookies.get('queuesmart_session');
    
    if (!sessionStr) {
        return { 
            user: { name: "Guest", userId: 0 }, 
            activeQueues: [] as (QueueEntry & { queue: Queue })[], 
            suggestedQueues: [] as Queue[] 
        };
    }

    const session = JSON.parse(sessionStr);
    const headers = {
        'Authorization': `Bearer ${session.token}`,
        'Content-Type': 'application/json'
    };

    const baseUrl = env.PUBLIC_API_URL || 'http://localhost:5201/api';

    try {
        const activeRes = await fetch(`${baseUrl}/queue/user/${session.userId}`, { 
            headers,
            cache: 'no-store' 
        });
        const rawActiveEntries = activeRes.ok ? await activeRes.json() : [];

        const suggestedRes = await fetch(`${baseUrl}/services`, { 
            headers,
            cache: 'no-store'
        });
        const rawServices = suggestedRes.ok ? await suggestedRes.json() : [];

        const suggestedQueues: Queue[] = rawServices.map((apiData: any) => ({
            ...apiData,
            status: apiData.isOpen ? "open" : "closed",
            currentLength: apiData.queueLength || 0,
            averageWaitTime: apiData.duration || 0,
            organization: apiData.organization || "QueueSmart Service",
            activeQueueId: apiData.activeQueueId 
        }));

        const activeQueues = rawActiveEntries.map((entry: any) => {
            const matchingQueue = suggestedQueues.find(q => 
                q.id === entry.serviceId || 
                q.id === entry.queueId ||
                (q as any).activeQueueId === entry.queueId
            );
            
            return {
                ...entry,
                queue: matchingQueue || null 
            };
        }).filter((entry: any) => entry.queue !== null); 

        return {
            user: session,
            activeQueues,
            suggestedQueues
        };
    } catch (error) {
        console.error("Failed to fetch data from .NET API:", error);
        return {
            user: session,
            activeQueues: [],
            suggestedQueues: []
        };
    }
};