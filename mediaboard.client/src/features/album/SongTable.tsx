import type { AlbumSong } from "@/api/albumApi";
import RatingBadge from "@/components/RatingBadge";
import NoRatingBadge from "@/components/NoRatingBadge";
import styles from "@/features/album/album.module.css";

const getMockRating = (missingChance: number) =>
	Math.random() < missingChance ? undefined : Math.random() * 9 + 1;

const SongTable = ({ songs }: { songs: AlbumSong[] }) => {
	return (
		<div className={styles.songTable}>
			{songs.map((song) => {
				const averageRating = getMockRating(0.15);
				const userRating = getMockRating(0.4);

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
							{averageRating != null ? (
								<RatingBadge rating={averageRating} />
							) : (
								<NoRatingBadge />
							)}
						</div>
						<p className={styles.userRating}>
							{userRating != null
								? (userRating / 2).toFixed(2)
								: "-"}
						</p>
					</div>
				);
			})}
		</div>
	);
};

export default SongTable;
