import { writable } from "svelte/store";
import {
	fetchServices as apiFetchServices,
	createService as apiCreateService,
	updateService as apiUpdateService,
	deleteService as apiDeleteService,
	type ServiceResponse,
	type ServiceRequest,
} from "$lib/api/services";

export const services = writable<ServiceResponse[]>([]);

export const servicesLoading = writable<boolean>(false);

export const servicesError = writable<string>("");

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

export async function addService(
	serviceData: ServiceRequest,
): Promise<ServiceResponse> {
	const created = await apiCreateService(serviceData);
	services.update(list => [...list, created]);
	return created;
}

export async function editService(
	id: string,
	serviceData: ServiceRequest,
): Promise<ServiceResponse> {
	const updated = await apiUpdateService(id, serviceData);
	services.update(list => list.map(s => (s.id === id ? updated : s)));
	return updated;
}

export async function removeService(id: string): Promise<void> {
	await apiDeleteService(id);
	services.update(list => list.filter(s => s.id !== id));
}

// Queue mock data (out of scope for this module)
export const queues = writable({
	1: [
		{
			id: 101,
			name: "Alice Johnson",
			joinedAt: "09:02 AM",
			status: "waiting",
		},
		{ id: 102, name: "Bob Smith", joinedAt: "09:10 AM", status: "waiting" },
		{
			id: 103,
			name: "Carol White",
			joinedAt: "09:18 AM",
			status: "waiting",
		},
	],
	2: [
		{ id: 201, name: "Dan Brown", joinedAt: "09:05 AM", status: "waiting" },
		{ id: 202, name: "Eve Davis", joinedAt: "09:22 AM", status: "waiting" },
	],
	3: [],
});
