import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Card";
import DefaultCover from "@/assets/music-album.svg?react";

const AlbumCard = ({
	title,
	year,
	imageSource,
	rating,
}: {
	title: string;
	year: string | undefined;
	imageSource?: string;
	rating?: number;
}) => {
	const imageElement = imageSource ? (
		<img src={imageSource} />
	) : (
		<DefaultCover stroke="currentColor" className={styles.defaultCover} />
	);

	return (
		<Card className={styles.albumCard}>
			<div className={styles.imageContainer}>{imageElement}</div>

			<div className={styles.info}>
				<div className={styles.headers}>
					<h2>{title}</h2>
					{year && <p>{year}</p>}
				</div>

				<div className={styles.rating}>{rating ?? "--"}</div>
			</div>
		</Card>
	);
};

export default AlbumCard;
