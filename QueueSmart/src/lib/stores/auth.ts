import { browser } from "$app/environment";
import { writable } from "svelte/store";

import type { AuthResponse } from "$lib/api/auth";

const STORAGE_KEY = "queuesmart.auth.session";

function readSession(): AuthResponse | null {
	if (!browser) {
		return null;
	}

	const stored = sessionStorage.getItem(STORAGE_KEY);

	if (!stored) {
		return null;
	}

	try {
		return JSON.parse(stored) as AuthResponse;
	} catch {
		sessionStorage.removeItem(STORAGE_KEY);
		return null;
	}
}

export const authSession = writable<AuthResponse | null>(readSession());

export function setAuthSession(session: AuthResponse): void {
	if (browser) {
		sessionStorage.setItem(STORAGE_KEY, JSON.stringify(session));
	}

	authSession.set(session);
}

export function clearAuthSession(): void {
	if (browser) {
		sessionStorage.removeItem(STORAGE_KEY);
	}

	authSession.set(null);
}
