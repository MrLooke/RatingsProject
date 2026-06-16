import "@/App.css";
import styles from "./home.module.css";
import HomeAlbumCard from "./HomeAlbumCard";
import TopArtistList from "./TopArtistList";
import { mockNewReleases } from "@/mock/homeData";

function Home() {
	return (
		<div className={styles.homeBody}>
			<div className={styles.newReleases}>
				<div className={styles.headerDiv}>
					<h4>New Releases</h4>
					<h5>See all</h5>
				</div>
				<div className={styles.albumGrid}>
					{mockNewReleases.map((album) => (
						<HomeAlbumCard
							key={`${album.title}-${album.artist}`}
							title={album.title}
							artist={album.artist}
							rating={album.rating}
						/>
					))}
				</div>
			</div>
			<div className={styles.topArtistListContainer}>
				<h4>Top Artists</h4>
				<TopArtistList />
			</div>
		</div>
	);
}

export default Home;
