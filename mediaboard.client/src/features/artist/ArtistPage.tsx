import { useState } from "react";
import styles from "@/features/artist/artist.module.css";
import ProfileImage from "@/components/ProfileImage";
import FullAlbumCard from "./FullAlbumCard";
import SongListItem from "./SongListItem";
import useArtistPage from "@/hooks/api/useArtistPage";
import { mockPopularSongs } from "@/mock/songData";

const FORMAT_FILTERS: Record<string, (format?: string | null) => boolean> = {
	ALL: () => true,
	ALBUMS: (format) => format === "Album",
	COMPILATIONS: (format) => format == "Compilation",
	EPS: (format) => format === "EP",
	SINGLES: (format) => format === "Single",
	MISC: (format) => !["Album", "EP", "Single"].includes(format ?? ""),
};

const FORMAT_HEADERS = Object.keys(FORMAT_FILTERS);

const ArtistPage = ({ artistId }: { artistId: number }) => {
	const { data, error, isPending, isError } = useArtistPage(artistId);
	const [selectedFormat, setSelectedFormat] = useState("ALL");

	if (artistId == null) {
		return (
			<div className={styles.artistBodyMessage}>Invalid artist ID.</div>
		);
	}

	if (isPending) {
		return <div className={styles.artistBodyMessage}>Loading...</div>;
	}

	if (isError || !data) {
		console.log(error?.message);
		return (
			<div className={styles.artistBodyMessage}>
				Error retrieving artist data.
			</div>
		);
	}

	const cleanArtistName = data.name.replace(/\s*\(\d+\)$/, "");

	const hasAlbums = data.albums.length > 0;
	const availableFormats = FORMAT_HEADERS.filter(
		(header) =>
			header === "ALL" ||
			data.albums.some((album) => FORMAT_FILTERS[header](album.format)),
	);

	return (
		<div className={styles.artistBody}>
			<div className={styles.mainColumn}>
				<h1 className={styles.artistName}>{cleanArtistName}</h1>

				{data.description && (
					<p className={styles.description}>{data.description}</p>
				)}

				{hasAlbums ? (
					<div className={styles.formatHeaders}>
						{availableFormats.flatMap((header, i) => [
							<h2
								key={header}
								className={
									selectedFormat === header
										? styles.active
										: undefined
								}
								onClick={() => setSelectedFormat(header)}
							>
								{header}
							</h2>,
							i < availableFormats.length - 1 ? (
								<h3 key={`${header}-separator`}>|</h3>
							) : null,
						])}
					</div>
				) : (
					<div className={styles.artistBodyMessage}>
						No releases yet.
					</div>
				)}

				<div>
					{data?.albums
						.filter((album) =>
							FORMAT_FILTERS[selectedFormat](album.format),
						)
						.map((album) => (
							<FullAlbumCard
								key={album.id}
								albumId={album.id}
								title={album.title}
								rating={album.averageRating}
								userRating={album.userRating}
								year={album.year?.toString()}
								format={album.format}
							/>
						))}
				</div>
			</div>

			<div className={styles.sidebar}>
				<ProfileImage
					alt={cleanArtistName + " artist image."}
					className={styles.artistImage}
				/>

				<div className={styles.songList}>
					<h2>Popular Songs</h2>
					{mockPopularSongs
						.sort((a, b) => b.rating - a.rating)
						.map((song) => (
							<SongListItem key={song.id} {...song} />
						))}
				</div>
			</div>
		</div>
	);
};

export default ArtistPage;
