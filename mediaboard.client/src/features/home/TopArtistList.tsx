import styles from "@/features/home/home.module.css";
import ArtistListItem from "./ArtistListItem";
import { mockTopArtists } from "@/mock/homeData";

const TopArtistList = () => {
	return (
		<ol className={styles.topArtistList}>
			{mockTopArtists.map((artist) => (
				<ArtistListItem
					key={artist.name}
					name={artist.name}
					albumCount={artist.albumCount}
					ratingCount={artist.ratingCount}
					averageRating={artist.averageRating}
					artistImage={artist.artistImage}
				/>
			))}
		</ol>
	);
};

export default TopArtistList;
