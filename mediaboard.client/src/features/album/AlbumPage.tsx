import useAlbumPage from "@/hooks/api/useAlbumPage";
import ArtistSmallCard from "@/components/Cards/ArtistSmallCard";
import styles from "@/features/album/album.module.css";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";
import Card from "@/components/Cards/Card";
import ListCard from "@/components/Lists/ListCard";
import ListHeader from "@/components/Lists/ListHeader";
import ListItem from "@/components/Lists/ListItem";
import SongTable from "@/features/album/SongTable";
import { Link } from "@tanstack/react-router";

const AlbumPage = ({ albumId }: { albumId: number }) => {
	const { data, error, isPending, isError } = useAlbumPage(albumId);

	const isEarlyReturn = albumId == null || isPending || isError;
	const bodyMessage = !isEarlyReturn
		? ""
		: isPending
			? "Loading..."
			: isError
				? "Error retrieving album data."
				: "Invalid album id.";

	if (isEarlyReturn || !data) {
		if (error) console.log(error);

		return <div className={styles.albumBody}>{bodyMessage}</div>;
	}

	const format = data.format ?? "Misc";
	const subHeader = (
		<>
			{data.year ? data.year + " · " : ""}
			{format}
			{data.artists.map((a) => {
				const artistLink = `/artist/${a.id}`;
				return (
					<>
						{" · "}
						<Link to={artistLink}>{a.name}</Link>
					</>
				);
			})}
		</>
	);

	return (
		<div className={styles.albumBody}>
			<div className={styles.mainColumn}>
				<div className={styles.headerContainer}>
					<h1>{data.title}</h1>
					<p className={styles.infoSubHeader}>{subHeader}</p>
				</div>
				<SongTable songs={data.songs} />
			</div>
			<div className={styles.sideBar}>
				<Card className={styles.coverImageCard}>
					<ImageWithDefault
						containerClass={styles.coverImage}
						src={data.imageUrl}
						alt={data.title + " Cover Photo"}
					>
						<DefaultAlbumCover />
					</ImageWithDefault>

					<div className="albumInfo">
						<div className={styles.infoItem}>
							<p className={styles.label}>Tracks</p>
							<p>{data.songs.length}</p>
						</div>
						<div className={styles.infoItem}>
							<p className={styles.label}>Ratings</p>
							<p>128</p>
						</div>
						<div className={styles.infoItem}>
							<p className={styles.label}>Average Rating</p>
							<p>4.4</p>
						</div>
					</div>
				</Card>

				<ListCard>
					<ListHeader alignment="center">Artists</ListHeader>
					{data?.artists.map((artist) => (
						<ListItem clickable>
							<ArtistSmallCard
								artistId={artist.id}
								name={artist.name}
							/>
						</ListItem>
					))}
				</ListCard>
			</div>
		</div>
	);
};

export default AlbumPage;
