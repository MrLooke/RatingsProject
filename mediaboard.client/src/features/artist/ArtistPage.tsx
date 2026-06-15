import ArtistHeader from "@/features/artist/ArtistHeader";
import styles from "@/features/artist/artist.module.css";
import ClairoImg from "@/assets/Clairo.jpg";
import AlbumCard from "./AlbumCard";
import SongListItem from "./SongListItem";
import useArtistPage from "@/hooks/api/useArtistPage";

const ArtistPage = ({ artistId }: { artistId: number }) => {
	if (artistId == null) {
		return (
			<div className={styles.artistBodyMessage}>Invalid artist ID.</div>
		);
	}

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

	const { data, error, isPending, isError } = useArtistPage(artistId);

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

	return (
		<div className={styles.artistBody}>
			<img src={ClairoImg} />
			<ArtistHeader>{cleanArtistName}</ArtistHeader>
			{data.description && (
				<p className={styles.description}>{data.description}</p>
			)}

			<div className={styles.albums}>
				{data?.albums.map((album) => (
					<AlbumCard
						key={album.id}
						title={album.title}
						year={album.year?.toString()}
					/>
				))}
			</div>
			<div className={styles.songList}>
				<h2>Popular Songs</h2>
				{songData
					.sort((a, b) => b.rating - a.rating)
					.map((song) => (
						<SongListItem {...song} />
					))}
			</div>
		</div>
	);
};

export default ArtistPage;
