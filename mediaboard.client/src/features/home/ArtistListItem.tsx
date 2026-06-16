import styles from "@/features/home/home.module.css";
import ProfileImage from "@/components/ProfileImage";

interface ArtistListItemProps {
	name: string;
	albumCount: number;
	ratingCount: number;
	averageRating: number;
	artistImage?: string | null;
}

const ArtistListItem = ({
	name,
	albumCount,
	averageRating,
	ratingCount,
	artistImage,
}: ArtistListItemProps) => {
	return (
		<li className={styles.artistListItem}>
			<ProfileImage
				src={artistImage}
				alt={`${name} Artist Image`}
				className={styles.artistImage}
			/>

			<div className={styles.artistInfo}>
				<h5>{name}</h5>
				<p>
					{albumCount} albums · {ratingCount} ratings
				</p>
			</div>

			<h6 className={styles.averageRating}>{averageRating}</h6>
		</li>
	);
};

export default ArtistListItem;
