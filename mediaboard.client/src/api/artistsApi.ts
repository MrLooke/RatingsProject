const apiUrl = import.meta.env.VITE_API_URL;

export interface ArtistSearchDTO {
	id: number;
	name: string;
	rankScore: number;
}

export const searchArtists = async (
	query: string,
	lastId: number = -1,
	lastRankScore: number = -1,
): Promise<ArtistSearchDTO[]> => {
	const response = await fetch(
		`${apiUrl}/artist?query=${query}&limit=15&lastId=${lastId}&lastRankScore=${lastRankScore}`,
	);

	if (!response.ok) {
		throw new Error("Error with search query.");
	}
	return response.json();
};
