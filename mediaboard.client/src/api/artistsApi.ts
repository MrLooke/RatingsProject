const apiUrl = import.meta.env.VITE_API_URL;

export interface ArtistSearchDTO {
	id: number;
	name: string;
	rankScore: number;
}

export const searchArtists = async (
	query: string,
	lastId: number | null = null,
	lastRankScore: number | null = null,
): Promise<ArtistSearchDTO[]> => {
	const params = new URLSearchParams({ query, limit: "15" });
	if (lastId !== null) params.append("lastId", String(lastId));
	if (lastRankScore !== null)
		params.append("lastRankScore", String(lastRankScore));

	const response = await fetch(`${apiUrl}/artist?${params}`);

	if (!response.ok) {
		throw new Error("Error with search query.");
	}
	return response.json();
};

interface Album {
	id: number;
	title: string;
	year?: number;
	format?: string | null;
}

export interface ArtistPage {
	id: number;
	name: string;
	description?: string;
	albums: Album[];
}

export const getArtistPage = async (artistId: number): Promise<ArtistPage> => {
	const response = await fetch(`${apiUrl}/artist/${artistId}`);

	if (!response.ok) {
		throw new Error("Error getting artist's information.");
	}

	return response.json();
};
