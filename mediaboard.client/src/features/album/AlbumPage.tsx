import useAlbumPage from "@/hooks/api/useAlbumPage";
import ArtistSmallCard from "@/components/Cards/ArtistSmallCard";
import styles from "@/features/album/album.module.css";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";
import ListCard from "@/components/Lists/ListCard";
import ListHeader from "@/components/Lists/ListHeader";
import ListItem from "@/components/Lists/ListItem";
import SongTable from "@/features/album/SongTable";

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

	return (
		<div className={styles.albumBody}>
			<div className={styles.mainColumn}>
				<h1>{data.title}</h1>
				<SongTable songs={data.songs} />
			</div>
			<div className={styles.sideBar}>
				<ImageWithDefault
					src={data.imageUrl}
					alt={data.title + " Cover Photo"}
				>
					<DefaultAlbumCover />
				</ImageWithDefault>
				<ListCard>
					<ListHeader alignment="center">Artists</ListHeader>
					{data?.artists.map((artist) => (
						<ListItem>
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
