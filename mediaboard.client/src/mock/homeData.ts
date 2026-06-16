export interface MockAlbum {
	title: string;
	artist: string;
	rating: number;
}

export interface MockArtist {
	name: string;
	albumCount: number;
	ratingCount: number;
	averageRating: number;
	artistImage?: string | null;
}

export const mockNewReleases: MockAlbum[] = [
	{ title: "Charm", artist: "Clairo", rating: 4.5 },
	{ title: "Short n' Sweet", artist: "Sabrina Carpenter", rating: 4.2 },
	{ title: "Radical Optimism", artist: "Dua Lipa", rating: 3.8 },
	{ title: "Hit Me Hard and Soft", artist: "Billie Eilish", rating: 4.7 },
];

export const mockTopArtists: MockArtist[] = [
	{ name: "Clairo", albumCount: 4, ratingCount: 100, averageRating: 4.1 },
	{ name: "Sabrina Carpenter", albumCount: 3, ratingCount: 85, averageRating: 3.9 },
	{ name: "Dua Lipa", albumCount: 5, ratingCount: 200, averageRating: 4.0 },
	{ name: "Billie Eilish", albumCount: 3, ratingCount: 150, averageRating: 4.3 },
	{ name: "Taylor Swift", albumCount: 11, ratingCount: 500, averageRating: 4.5 },
];
