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

async function handleResponse<T>(res: Response): Promise<T> {
	if (!res.ok) {
		const body = await res.json().catch(() => null);
		const message =
			body?.title || body?.message || `Request failed (${res.status})`;
		throw new Error(message);
	}
	return res.json();
}

export async function fetchServices(): Promise<ServiceResponse[]> {
	const res = await fetch(`${BASE}/services`);
	return handleResponse<ServiceResponse[]>(res);
}

export async function createService(
	data: ServiceRequest,
): Promise<ServiceResponse> {
	const res = await fetch(`${BASE}/services`, {
		method: "POST",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify(data),
	});
	return handleResponse<ServiceResponse>(res);
}

export async function updateService(
	id: string,
	data: ServiceRequest,
): Promise<ServiceResponse> {
	const res = await fetch(`${BASE}/services/${id}`, {
		method: "PUT",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify(data),
	});
	return handleResponse<ServiceResponse>(res);
}

export async function deleteService(id: string): Promise<void> {
	const res = await fetch(`${BASE}/services/${id}`, { method: "DELETE" });
	if (!res.ok) {
		const body = await res.json().catch(() => null);
		throw new Error(body?.title || `Delete failed (${res.status})`);
	}
}

export async function fetchHistory(
	serviceId?: string,
): Promise<HistoryEntry[]> {
	const url = serviceId
		? `${BASE}/history?serviceId=${serviceId}`
		: `${BASE}/history`;
	const res = await fetch(url);
	return handleResponse<HistoryEntry[]>(res);
}

export async function fetchStatistics(): Promise<StatisticsResponse> {
	const res = await fetch(`${BASE}/history/statistics`);
	return handleResponse<StatisticsResponse>(res);
}
