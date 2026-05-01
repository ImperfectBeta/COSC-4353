import { browser } from "$app/environment";
import { writable } from "svelte/store";
import type { AuthResponse } from "$lib/api/auth";

const COOKIE_NAME = "queuesmart_session";

function getCookie(name: string): string | null {
    if (!browser) return null;
    const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    return match ? decodeURIComponent(match[2]) : null;
}

function setCookie(name: string, value: string, days: number = 7) {
    if (!browser) return;
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    document.cookie = `${name}=${encodeURIComponent(value)};expires=${date.toUTCString()};path=/;SameSite=Strict`;
}

function deleteCookie(name: string) {
    if (!browser) return;
    document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/`;
}

function readSession(): AuthResponse | null {
    const stored = getCookie(COOKIE_NAME);
    if (!stored) return null;
    
    try {
        return JSON.parse(stored) as AuthResponse;
    } catch {
        deleteCookie(COOKIE_NAME);
        return null;
    }
}

export const authSession = writable<AuthResponse | null>(readSession());

export function setAuthSession(session: AuthResponse): void {
    if (browser) {
        setCookie(COOKIE_NAME, JSON.stringify(session));
    }
    authSession.set(session);
}

export function clearAuthSession(): void {
    if (browser) {
        deleteCookie(COOKIE_NAME);
    }
    authSession.set(null);
}