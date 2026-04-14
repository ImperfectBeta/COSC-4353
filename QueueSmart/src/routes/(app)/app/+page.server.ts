import type { PageServerLoad } from "./$types";
import type { Queue, QueueEntry } from "$lib/types";

export const load: PageServerLoad = async () => {
    // will fetch from actual API

    return {
        user: { id: "temp", name: "User", email: "", role: "user" },
        
        activeQueues: [] as (QueueEntry & { queue: Queue })[],
        suggestedQueues: [] as Queue[], 
    };
};