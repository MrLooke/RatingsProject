import styles from "./home.module.css";
import componentStyles from "@/components/components.module.css";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";
import RatingBadge from "@/components/RatingBadge";

interface HomeAlbumCardProps {
	title: string;
	artist: string;
	rating: number;
}

const HomeAlbumCard = ({ title, artist, rating }: HomeAlbumCardProps) => {
	return (
		<div className={styles.albumCard}>
			<ImageWithDefault
				alt={title + " Cover Image"}
				containerClass={styles.albumImgContainer}
			>
				<DefaultAlbumCover />
			</ImageWithDefault>
			<div className={componentStyles.footer}>
				<h2>{title}</h2>
				<h3>{artist}</h3>
			</div>
			<RatingBadge rating={rating} />
		</div>
	);
};

export default HomeAlbumCard;
