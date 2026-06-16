import styles from "@/features/home/home.module.css";
import ArtistListItem from "./ArtistListItem";

const TopArtistList = () => {
	return (
		<ol className={styles.topArtistList}>
			<ArtistListItem
				name="Clairo"
				albumCount={4}
				ratingCount={100}
				averageRating={4.1}
			/>
			<ArtistListItem
				name="Clairo"
				albumCount={4}
				ratingCount={100}
				averageRating={4.1}
			/>
			<ArtistListItem
				name="Clairo"
				albumCount={4}
				ratingCount={100}
				averageRating={4.1}
			/>{" "}
			<ArtistListItem
				name="Clairo"
				albumCount={4}
				ratingCount={100}
				averageRating={4.1}
			/>{" "}
			<ArtistListItem
				name="Clairo"
				albumCount={4}
				ratingCount={100}
				averageRating={4.1}
			/>
		</ol>
	);
};

export default TopArtistList;
