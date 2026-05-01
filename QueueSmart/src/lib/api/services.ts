import { get } from "svelte/store";
import { authSession } from "$lib/stores/auth";

const BASE = "/api";

export interface ServiceRequest {
    name: string;
    description: string;
    duration: number;
    priority: string;
}

export interface ServiceResponse {
    id: string;
    name: string;
    description: string;
    duration: number;
    priority: string;
    isOpen: boolean;
    queueLength: number;
    createdAt: string;
    updatedAt: string;
}

export interface HistoryEntry {
    id: string;
    serviceId: string;
    serviceName: string;
    action: string;
    timestamp: string;
    details: string | null;
}

export interface StatisticsResponse {
    totalServices: number;
    activeServices: number;
    totalHistoryEntries: number;
    recentHistory: HistoryEntry[];
}

export interface JoinQueueRequest {
    userId: number;
    queueId: string;
    priority: number;
}

export interface QueueEntryResponse {
    id: number;
    userId: number;
    serviceId: number;
    joinedAt: string;
    priority: number;
    status: string;
    position: number;
    estimatedWaitMinutes: number;
}

function getAuthHeaders(): Record<string, string> {
    const headers: Record<string, string> = {
        "Content-Type": "application/json"
    };
    
    const session = get(authSession);
    if (session?.token) {
        headers["Authorization"] = `Bearer ${session.token}`;
    }
    
    return headers;
}

async function handleResponse<T>(res: Response): Promise<T> {
    if (!res.ok) {
        const body = await res.json().catch(() => null);
        let message = body?.title || body?.message || `Request failed (${res.status})`;

        if (body?.errors && typeof body.errors === 'object') {
            const errorDetails = Object.entries(body.errors)
                .map(([field, messages]) => `${field}: ${(messages as string[]).join(', ')}`)
                .join(' | ');
            message = `Validation Failed - ${errorDetails}`;
        }

        throw new Error(message);
    }
    return res.json();
}

export async function fetchServices(): Promise<ServiceResponse[]> {
    const res = await fetch(`${BASE}/services`, { 
        headers: getAuthHeaders() 
    });
    return handleResponse<ServiceResponse[]>(res);
}

export async function createService(data: ServiceRequest): Promise<ServiceResponse> {
    const res = await fetch(`${BASE}/services`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify(data),
    });
    return handleResponse<ServiceResponse>(res);
}

export async function updateService(id: string, data: ServiceRequest): Promise<ServiceResponse> {
    const res = await fetch(`${BASE}/services/${id}`, {
        method: "PUT",
        headers: getAuthHeaders(),
        body: JSON.stringify(data),
    });
    return handleResponse<ServiceResponse>(res);
}

export async function deleteService(id: string): Promise<void> {
    const res = await fetch(`${BASE}/services/${id}`, { 
        method: "DELETE",
        headers: getAuthHeaders()
    });
    if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.title || `Delete failed (${res.status})`);
    }
}

export async function fetchHistory(serviceId?: string): Promise<HistoryEntry[]> {
    const url = serviceId
        ? `${BASE}/history?serviceId=${serviceId}`
        : `${BASE}/history`;
    const res = await fetch(url, { headers: getAuthHeaders() });
    return handleResponse<HistoryEntry[]>(res);
}

export async function fetchStatistics(): Promise<StatisticsResponse> {
    const res = await fetch(`${BASE}/history/statistics`, { 
        headers: getAuthHeaders() 
    });
    return handleResponse<StatisticsResponse>(res);
}

export async function joinQueue(data: JoinQueueRequest): Promise<QueueEntryResponse> {
    const res = await fetch(`${BASE}/queue/join`, {
        method: "POST",
        headers: getAuthHeaders(),
        body: JSON.stringify(data),
    });
    return handleResponse<QueueEntryResponse>(res);
}

export async function fetchUserNotifications(userId: number): Promise<string[]> {
    const res = await fetch(`${BASE}/notifications/${userId}`, { 
        headers: getAuthHeaders() 
    });
    return handleResponse<string[]>(res);
}

export interface LeaveQueueRequest {
    userId: number;
    entryId: number;
}

export async function leaveQueue(data: LeaveQueueRequest): Promise<void> {
    const res = await fetch(`${BASE}/queue/leave/${data.entryId}?userId=${data.userId}`, {
        method: "DELETE", 
        headers: getAuthHeaders(),
    });
    
    if (!res.ok) {
        const body = await res.json().catch(() => null);
        let message = body?.title || body?.message || `Failed to leave queue (${res.status})`;
        throw new Error(message);
    }
}