import { writable } from "svelte/store";
import {
    fetchServices as apiFetchServices,
    createService as apiCreateService,
    updateService as apiUpdateService,
    deleteService as apiDeleteService,
    fetchQueueEntries as apiFetchQueueEntries,
    serveNextUser as apiServeNextUser,
    adminRemoveUserFromQueue as apiAdminRemoveUser,
    reorderQueueEntries as apiReorderQueueEntries,
    type ServiceResponse,
    type ServiceRequest,
} from "$lib/api/services";

export const services = writable<ServiceResponse[]>([]);
export const servicesLoading = writable<boolean>(false);
export const servicesError = writable<string>("");

export const activeQueueEntries = writable<any[]>([]);
export const activeQueueId = writable<string | null>(null);

export async function loadServices() {
    servicesLoading.set(true);
    servicesError.set("");
    try {
        const data = await apiFetchServices();
        services.set(data);
    } catch (err) {
        servicesError.set((err as Error).message || "Failed to load services");
    } finally {
        servicesLoading.set(false);
    }
}

export async function addService(serviceData: ServiceRequest): Promise<ServiceResponse> {
    const created = await apiCreateService(serviceData);
    services.update(list => [...list, created]);
    return created;
}

export async function editService(id: string, serviceData: ServiceRequest): Promise<ServiceResponse> {
    const updated = await apiUpdateService(id, serviceData);
    services.update(list => list.map(s => (s.id === id ? updated : s)));
    return updated;
}

export async function removeService(id: string): Promise<void> {
    await apiDeleteService(id);
    services.update(list => list.filter(s => s.id !== id));
}

export async function loadQueueEntries(serviceId: string) {
    try {
        const currentServices = await apiFetchServices();
        const targetService = currentServices.find(s => s.id === serviceId);
        
        if (!targetService || !targetService.isOpen || !targetService.activeQueueId) {
            activeQueueEntries.set([]);
            activeQueueId.set(null);
            return;
        }

        const exactQueueId = targetService.activeQueueId;
        activeQueueId.set(exactQueueId);

        const entries = await apiFetchQueueEntries(exactQueueId);
        activeQueueEntries.set(entries);
        
    } catch (err) {
        console.error("Failed to load queue entries:", err);
        activeQueueEntries.set([]);
        activeQueueId.set(null);
    }
}

export async function processNextUser(queueId: string) {
    try {
        await apiServeNextUser(queueId);
        const updatedEntries = await apiFetchQueueEntries(queueId);
        activeQueueEntries.set(updatedEntries);
    } catch (err) {
        console.error("Failed to serve next user:", err);
        alert("Error serving next user. Please try again.");
    }
}

export async function dropUserFromQueue(queueId: string, entryId: number, userId: number) {
    try {
        await apiAdminRemoveUser(entryId, userId);
        const updatedEntries = await apiFetchQueueEntries(queueId);
        activeQueueEntries.set(updatedEntries);
    } catch (err) {
        console.error("Failed to remove user:", err);
        alert("Error removing user. Please try again.");
    }
}

// Maps the reordered entries to IDs
export async function saveQueueOrder(queueId: string, orderedEntries: any[]) {
    const orderedEntryIds = orderedEntries.map(e => e.id);
    try {
        await apiReorderQueueEntries(queueId, orderedEntryIds);
    } catch (err) {
        console.error("Reorder failed, resetting UI:", err);
        loadQueueEntries(queueId); 
    }
}