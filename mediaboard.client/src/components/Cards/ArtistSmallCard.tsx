import { Link } from "@tanstack/react-router";
import styles from "@/components/components.module.css";
import ProfileImage from "../ProfileImage";

interface ArtistCardProps extends React.HTMLAttributes<HTMLDivElement> {
	artistId: number;
	name: string;
	imageUrl?: string;
}

const ArtistSmallCard = ({
	artistId,
	name,
	imageUrl,
	...rest
}: ArtistCardProps) => {
	const artistLink = `/artist/${artistId}`;

	return (
		<Link to={artistLink}>
			<div className={styles.artistSmallCard} {...rest}>
				<ProfileImage src={imageUrl} alt={name + " Artist Photo"} />
				<p>{name}</p>
			</div>
		</Link>
	);
};

export default ArtistSmallCard;
