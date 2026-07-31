import useAlbumPage from "@/hooks/api/useAlbumPage";
import styles from "@/features/album/album.module.css";

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

	if (isEarlyReturn) {
		if (error) console.log(error);

		return <div className={styles.albumBody}>{bodyMessage}</div>;
	}

	return (
		<div className={styles.albumBody}>
			<div className={styles.mainColumn}>{JSON.stringify(data)}</div>
			<div className={styles.sidebar}></div>
		</div>
	);
};

export default AlbumPage;
