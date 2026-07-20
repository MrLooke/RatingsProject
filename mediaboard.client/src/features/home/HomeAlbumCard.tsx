import styles from "./home.module.css";
import ImageCard from "../../components/Cards/ImageCard";
import RatingBadge from "@/components/RatingBadge";

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
			<RatingBadge rating={rating} />
		</ImageCard>
	);
};

export default HomeAlbumCard;
