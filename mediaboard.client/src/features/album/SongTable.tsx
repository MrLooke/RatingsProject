import type { AlbumSong } from "@/api/albumApi";
import RatingBadge from "@/components/RatingBadge";
import NoRatingBadge from "@/components/NoRatingBadge";
import styles from "@/features/album/album.module.css";

const SongTable = ({ songs }: { songs: AlbumSong[] }) => {
	return (
		<div className={styles.songTable}>
			{songs.length > 0 && (
				<div className={styles.songTableHeader}>
					<p className={styles.tracklistHeader}>Tracklist</p>
					<p className={styles.avgHeader}>Avg</p>
					<p className={styles.youHeader}>You</p>
				</div>
			)}
			{songs.map((song) => {
				return (
					<div className={styles.songRow} key={song.id}>
						<p className={styles.trackNumber}>{song.trackNumber}</p>
						<div className={styles.songTitleCell}>
							<p className={styles.songTitle} title={song.title}>
								{song.title}
							</p>
							{song.duration && (
								<p className={styles.songDuration}>
									{song.duration}
								</p>
							)}
						</div>
						<div className={styles.averageRatingCell}>
							{song.averageRating != null ? (
								<RatingBadge rating={song.averageRating} />
							) : (
								<NoRatingBadge />
							)}
						</div>
						<p className={styles.userRating}>
							{song.userRating != null
								? (song.userRating / 2).toFixed(2)
								: "-"}
						</p>
					</div>
				);
			})}
		</div>
	);
};

export default SongTable;
