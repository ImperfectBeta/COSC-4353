import { get } from "svelte/store";
import { authSession } from "$lib/stores/auth";

const BASE = "/api";

export interface ReportStatistics {
    totalUsersJoined: number;
    totalUsersServed: number;
    totalCancelled: number;
    mostActiveService: string;
    averageWaitTimeMinutes: number;
}

export interface ReportFilters {
    startDate?: string;
    endDate?: string;
    serviceIds?: string[];
    status?: string;
    minWait?: number;
    maxWait?: number;
}

function getAuthHeaders(): Record<string, string> {
    const headers: Record<string, string> = {
        "Content-Type": "application/json"
    };
    const session = get(authSession);
    if (session?.token) {
        headers["Authorization"] = `Bearer ${session.token}`;
        if (session.userId) {
            headers["X-User-Id"] = session.userId.toString();
        }
    }
    return headers;
}

function buildQueryString(filters: ReportFilters): string {
    const query = new URLSearchParams();
    if (filters.startDate) query.append("startDate", filters.startDate);
    if (filters.endDate) query.append("endDate", filters.endDate);
    if (filters.status) query.append("status", filters.status);
    if (filters.minWait) query.append("minWait", filters.minWait.toString());
    if (filters.maxWait) query.append("maxWait", filters.maxWait.toString());
    
    // Append multiple service IDs
    if (filters.serviceIds && filters.serviceIds.length > 0) {
        filters.serviceIds.forEach(id => query.append("serviceIds", id));
    }
    
    return query.toString();
}

export async function fetchReportStatistics(filters: ReportFilters): Promise<ReportStatistics> {
    const qs = buildQueryString(filters);
    const res = await fetch(`${BASE}/reports/statistics?${qs}`, { headers: getAuthHeaders() });
    
    if (!res.ok) throw new Error("Failed to load statistics");
    return res.json();
}

export async function downloadPdfReport(filters: ReportFilters): Promise<void> {
    const qs = buildQueryString(filters);
    const res = await fetch(`${BASE}/reports/export/pdf?${qs}`, { headers: getAuthHeaders() });
    
    if (!res.ok) throw new Error("Failed to download report");
    
    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `QueueSmart_MasterReport_${new Date().toISOString().slice(0, 10)}.pdf`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
}