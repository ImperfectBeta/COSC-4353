import { env } from "$env/dynamic/private";
import { json } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";

const GEMINI_MODEL = "gemini-3-flash-preview";
const GEMINI_URL = `https://generativelanguage.googleapis.com/v1beta/models/${GEMINI_MODEL}:generateContent`;
const API_BASE = "http://localhost:5201/api";

export const POST: RequestHandler = async ({ request }) => {
	const { message, userId, history } = await request.json();

	const apiKey = env.GEMINI_API_KEY;
	if (!apiKey) {
		return json(
			{
				reply: "AI assistant is not configured. Please set the GEMINI_API_KEY environment variable.",
			},
			{ status: 500 },
		);
	}

	// Fetch live queue data from .NET API
	let servicesData: any[] = [];
	let userEntries: any[] = [];

	try {
		const servicesRes = await fetch(`${API_BASE}/services`);
		if (servicesRes.ok) servicesData = await servicesRes.json();
	} catch {
		/* .NET API might be down */
	}

	try {
		if (userId) {
			const entriesRes = await fetch(`${API_BASE}/queue/user/${userId}`);
			if (entriesRes.ok) userEntries = await entriesRes.json();
		}
	} catch {
		/* ignore */
	}

	// Build system prompt with real-time context
	const systemPrompt = buildSystemPrompt(servicesData, userEntries);

	// Build Gemini conversation contents (multi-turn)
	const contents: Array<{ role: string; parts: Array<{ text: string }> }> =
		[];

	if (Array.isArray(history)) {
		for (const msg of history) {
			contents.push({
				role: msg.role === "assistant" ? "model" : "user",
				parts: [{ text: msg.text }],
			});
		}
	}

	contents.push({ role: "user", parts: [{ text: message }] });

	try {
		const geminiRes = await fetch(`${GEMINI_URL}?key=${apiKey}`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify({
				systemInstruction: { parts: [{ text: systemPrompt }] },
				contents,
			}),
		});

		if (!geminiRes.ok) {
			const errorBody = await geminiRes.text();
			console.error("Gemini API error:", geminiRes.status, errorBody);
			return json({
				reply: "Sorry, I couldn't process that right now. Please try again in a moment.",
			});
		}

		const geminiData = await geminiRes.json();
		const reply =
			geminiData.candidates?.[0]?.content?.parts?.[0]?.text ||
			"I wasn't able to generate a response. Please try again.";

		return json({ reply });
	} catch (err) {
		console.error("Gemini fetch error:", err);
		return json({
			reply: "Sorry, something went wrong connecting to the AI service.",
		});
	}
};

function buildSystemPrompt(services: any[], userEntries: any[]): string {
	let prompt = `You are QueueSmart AI, a friendly and concise assistant for a queue management application called QueueSmart.
You help users understand queue wait times, recommend which queues to join, and answer questions about available services.

Rules:
- Keep responses to 2-3 sentences max. Be concise and helpful.
- Use the live data below to give accurate, real-time answers.
- If you don't have enough data to answer, say so honestly.
- Never make up queue lengths or wait times — only use the data provided.
- Format wait times in a human-friendly way (e.g. "about 20 minutes").

LIVE QUEUE DATA:
`;

	if (services.length > 0) {
		prompt += "\nAvailable Services:\n";
		for (const s of services) {
			const status = s.isOpen ? "Open" : "Closed";
			const waiting = s.queueLength ?? 0;
			const estWait = waiting * (s.duration || 10);
			prompt += `- "${s.name}": ${s.description || "No description"}. ~${s.duration || "?"} min per person. Status: ${status}. People waiting: ${waiting}. Est. total wait: ~${estWait} min.\n`;
		}
	} else {
		prompt += "\nNo services are currently available.\n";
	}

	if (userEntries.length > 0) {
		prompt += "\nThis user's current queue positions:\n";
		for (const e of userEntries) {
			prompt += `- Position #${e.position} in a queue. Estimated wait: ~${e.estimatedWaitMinutes} min. Status: ${e.status}.\n`;
		}
	} else {
		prompt += "\nThis user is not currently in any queues.\n";
	}

	return prompt;
}
