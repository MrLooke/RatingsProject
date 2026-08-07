import styles from "@/features/artist/artist.module.css";

interface SongData {
	id: number;
	title: string;
	album: string;
	rating?: number;
}

const SongListItem = (data: SongData) => {
	return (
		<div className={styles.songItem}>
			<h3>{data.title}</h3>
			<h4 title={data.album}>{data.album}</h4>
			<div className={styles.songRating}>{data.rating ?? "-"}</div>
		</div>
	);
};

export default SongListItem;
