const apiUrl = import.meta.env.VITE_API_URL;

export interface AlbumSong {
	id: number;
	title: string;
	trackNumber: number;
	position?: string;
	duration?: string;
	averageRating?: number;
	userRating?: number;
}

export interface AlbumArtist {
	id: number;
	name: string;
}

export interface AlbumPageDTO {
	id: number;
	title: string;
	artists: AlbumArtist[];
	songs: AlbumSong[];
	year?: number;
	format?: string;
	imageUrl?: string;
}

export const getAlbumPage = async (albumId: number): Promise<AlbumPageDTO> => {
	const response = await fetch(`${apiUrl}/album/${albumId}`, {
		credentials: "include",
	});

	if (!response.ok) {
		throw new Error("Error getting album information.");
	}

	return response.json();
};
