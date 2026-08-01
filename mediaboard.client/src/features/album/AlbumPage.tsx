import useAlbumPage from "@/hooks/api/useAlbumPage";
import Card from "@/components/Cards/Card";
import ArtistSmallCard from "@/components/Cards/ArtistSmallCard";
import styles from "@/features/album/album.module.css";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";

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
				{JSON.stringify(data)}
			</div>
			<div className={styles.sideBar}>
				<ImageWithDefault
					src={data.imageUrl}
					alt={data.title + " Cover Photo"}
				>
					<DefaultAlbumCover />
				</ImageWithDefault>
				<Card>
					{" "}
					{data?.artists.map((artist) => (
						<ArtistSmallCard
							artistId={artist.id}
							name={artist.name}
						/>
					))}
				</Card>
			</div>
		</div>
	);
};

export default AlbumPage;
