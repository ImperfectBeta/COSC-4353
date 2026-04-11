const BASE = "/api/auth";

export type AuthRole = "User" | "ServiceAdmin" | "SystemAdmin";

export type RegisterRole = "User" | "ServiceAdmin";

export interface RegisterRequest {
	name: string;
	email: string;
	password: string;
	role: RegisterRole;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface AuthResponse {
	userId: number;
	name: string;
	email: string;
	role: AuthRole;
	token: string;
}

async function handleResponse<T>(res: Response): Promise<T> {
	const body = await res.json().catch(() => null);

	if (!res.ok) {
		throw new Error(body?.message || `Request failed (${res.status})`);
	}

	return body as T;
}

export async function registerUser(data: RegisterRequest): Promise<AuthResponse> {
	const res = await fetch(`${BASE}/register`, {
		method: "POST",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify(data),
	});

	return handleResponse<AuthResponse>(res);
}

export async function loginUser(data: LoginRequest): Promise<AuthResponse> {
	const res = await fetch(`${BASE}/login`, {
		method: "POST",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify(data),
	});

	return handleResponse<AuthResponse>(res);
}

export function getAuthDestination(role: AuthRole): string {
	return role === "User" ? "/app" : "/admin";
}

export const REGISTER_ROLE_OPTIONS = [
	{ label: "Basic User", value: "User" as const },
	{ label: "Administrator (Organization)", value: "ServiceAdmin" as const },
] as const;
