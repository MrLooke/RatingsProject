import { apiFetch } from "@/api/client";

const apiUrl = import.meta.env.VITE_API_URL;

export const submitRating = async (
	albumId: number,
	score: number,
	review?: string,
): Promise<void> => {
	const response = await apiFetch(`${apiUrl}/rating`, {
		method: "PUT",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify({ albumId, score, review }),
	});

	if (!response.ok) {
		const error = await response.json();
		throw new Error(error.error ?? "Failed to submit rating.");
	}
};
