import "@/App.css";
import styles from "./home.module.css";
import ImageCard from "../../components/Cards/ImageCard";
import HomeAlbumCard from "./HomeAlbumCard";
import TopArtistList from "./TopArtistList";

function Home() {
	return (
		<div className={styles.homeBody}>
			<div className={styles.newReleases}>
				<div className={styles.headerDiv}>
					<h4>New Releases</h4>
					<h5>See all</h5>
				</div>
				<div className={styles.albumGrid}>
					<HomeAlbumCard title="Charm" artist="Clairo" rating={4.5} />
					<HomeAlbumCard title="Charm" artist="Clairo" rating={4.5} />
					<HomeAlbumCard title="Charm" artist="Clairo" rating={4.5} />
					<HomeAlbumCard title="Charm" artist="Clairo" rating={4.5} />
				</div>
			</div>
			<div className={styles.topArtistListContainer}>
				<h4
					style={{
						textAlign: "left",
						marginTop: "40px",
						marginBottom: "10px",
					}}
				>
					Top Artists
				</h4>
				<TopArtistList />
			</div>
		</div>
	);
}

export default Home;
