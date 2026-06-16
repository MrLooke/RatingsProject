import "@/App.css";
import styles from "@/features/home/home.module.css";
import Star from "@/assets/star.svg?react";
import ImageCard from "../../components/Cards/ImageCard";

interface HomeAlbumCardProps {
	title: string;
	artist: string;
	rating: number;
}
const HomeAlbumCard = ({ title, artist, rating }: HomeAlbumCardProps) => {
	return (
		<ImageCard
			header={title}
			subheader={artist}
			className={styles.albumCard}
			imageContainerClass={styles.albumImgContainer}
		>
			<div className={styles.rating}>
				<Star stroke="currentColor" />
				<p>{rating}</p>
			</div>
		</ImageCard>
	);
};

export default HomeAlbumCard;
