import { useState } from "react";
import styles from "@/features/artist/artist.module.css";
import ClairoImg from "@/assets/Clairo.jpg";
import ProfileImage from "@/components/ProfileImage";
import ExpandableText from "@/components/ExpandableText";
import FullAlbumCard from "./FullAlbumCard";
import SongListItem from "./SongListItem";
import useArtistPage from "@/hooks/api/useArtistPage";

const FORMAT_FILTERS: Record<string, (format?: string | null) => boolean> = {
	ALL: () => true,
	ALBUMS: (format) => format === "Album",
	COMPILATIONS: (format) => format == "Compilation",
	EPS: (format) => format === "EP",
	SINGLES: (format) => format === "Single",
	MISC: (format) => !["Album", "EP", "Single"].includes(format ?? ""),
};

const FORMAT_HEADERS = Object.keys(FORMAT_FILTERS);

const songData = [
	{
		id: "song_01",
		title: "Sofia",
		album: "Immunity",
		rating: 4.9,
	},
	{
		id: "song_02",
		title: "Bags",
		album: "Immunity",
		rating: 4.8,
	},
	{
		id: "song_03",
		title: "Pretty Girl",
		album: "diary 001",
		rating: 4.7,
	},
	{
		id: "song_04",
		title: "Juna",
		album: "Charm",
		rating: 4.6,
	},
	{
		id: "song_05",
		title: "Sexy to Someone",
		album: "Charm",
		rating: 4.5,
	},
	{
		id: "song_06",
		title: "Amoeba",
		album: "Sling",
		rating: 4.4,
	},
	{
		id: "song_07",
		title: "Flaming Hot Cheetos",
		album: "diary 001",
		rating: 4.3,
	},
	{
		id: "song_08",
		title: "Blouse",
		album: "Sling",
		rating: 4.2,
	},
	{
		id: "song_09",
		title: "4EVER",
		album: "Single",
		rating: 4.6,
	},
	{
		id: "song_10",
		title: "Bubble Gum",
		album: "Single",
		rating: 4.3,
	},
];

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
					<ExpandableText
						className={styles.description}
						lineClamp={5}
					>
						<p>{data.description}</p>
					</ExpandableText>
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
								year={album.year?.toString()}
								format={album.format}
							/>
						))}
				</div>
			</div>

			<div className={styles.sidebar}>
				<ProfileImage
					src={ClairoImg}
					alt={cleanArtistName + " artist image."}
					className={styles.artistImage}
				/>

				<div className={styles.songList}>
					<h2>Popular Songs</h2>
					{songData
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
