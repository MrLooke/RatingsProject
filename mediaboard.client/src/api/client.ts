const apiUrl = import.meta.env.VITE_API_URL;

let refreshPromise: Promise<boolean> | null = null;

const refreshAccessToken = (): Promise<boolean> => {
	if (!refreshPromise) {
		refreshPromise = fetch(`${apiUrl}/auth/refresh`, {
			method: "POST",
			credentials: "include",
		})
			.then((response) => response.ok)
			.catch(() => false)
			.finally(() => {
				refreshPromise = null;
			});
	}

	return refreshPromise;
};

export const apiFetch = async (
	input: string,
	init: RequestInit = {},
): Promise<Response> => {
	const request = () => fetch(input, { ...init, credentials: "include" });

	const response = await request();
	if (response.status !== 401) {
		return response;
	}

	const refreshed = await refreshAccessToken();
	if (!refreshed) {
		return response;
	}

	return request();
};
