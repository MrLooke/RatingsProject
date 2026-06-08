const apiUrl = import.meta.env.VITE_API_URL;

interface ArtistSearchDTO {
	id: number;
	name: string;
	description: string;
}

export const searchArtists = async (
	query: string,
): Promise<ArtistSearchDTO[]> => {
	const response = await fetch(`${apiUrl}/artist?query=${query}&limit=15`);

	if (!response.ok) {
		throw new Error("Error with search query.");
	}
	return response.json();
};
