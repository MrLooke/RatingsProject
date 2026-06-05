import ArtistHeader from "@/features/artist/ArtistHeader";
import styles from "@/features/artist/artist.module.css";
import ClairoImg from "@/assets/Clairo.jpg";
import AlbumCard from "./AlbumCard";
import SongListItem from "./SongListItem";

const ArtistPage = () => {
    const albumData = [
        {
            id: "album_01",
            imageSource:
                "https://en.wikipedia.org/wiki/Special:FilePath/Clairo_-_Immunity.png",
            title: "Immunity",
            date: "August 2, 2019",
            rating: 4.5,
        },
        {
            id: "album_02",
            imageSource:
                "https://en.wikipedia.org/wiki/Special:FilePath/Clairo_-_Sling.png",
            title: "Sling",
            date: "July 16, 2021",
            rating: 4.8,
        },
        {
            id: "album_03",
            imageSource:
                "https://upload.wikimedia.org/wikipedia/en/thumb/d/dc/Clairo_-_Charm.png/250px-Clairo_-_Charm.png",
            title: "Charm",
            date: "July 12, 2024",
            rating: 4.2,
        },
    ];

    const songData = [
        {
            id: "song_01",
            title: "Sofia",
            album: "Immunity",
            rating: 4.9,
        },
        {
            id: "song_02",
            title: "Bags",
            album: "Immunity",
            rating: 4.8,
        },
        {
            id: "song_03",
            title: "Pretty Girl",
            album: "diary 001",
            rating: 4.7,
        },
        {
            id: "song_04",
            title: "Juna",
            album: "Charm",
            rating: 4.6,
        },
        {
            id: "song_05",
            title: "Sexy to Someone",
            album: "Charm",
            rating: 4.5,
        },
        {
            id: "song_06",
            title: "Amoeba",
            album: "Sling",
            rating: 4.4,
        },
        {
            id: "song_07",
            title: "Flaming Hot Cheetos",
            album: "diary 001",
            rating: 4.3,
        },
        {
            id: "song_08",
            title: "Blouse",
            album: "Sling",
            rating: 4.2,
        },
        {
            id: "song_09",
            title: "4EVER",
            album: "Single",
            rating: 4.6,
        },
        {
            id: "song_10",
            title: "Bubble Gum",
            album: "Single",
            rating: 4.3,
        },
    ];

    return (
        <div className={styles.artistBody}>
            <img src={ClairoImg} />
            <ArtistHeader>Clairo</ArtistHeader>
            <p className={styles.description}>
                Clairo (Claire Cottrill) is an American singer-songwriter and
                producer who rose to fame as a defining voice of the bedroom pop
                genre before evolving into a more mature indie-pop artist. Known
                for her intimate, diary-like songwriting and soft vocals, she
                has transitioned from lo-fi bedroom recordings to lush,
                70s-influenced soul and folk-pop on albums like Immunity (2019)
                and Sling (2021).
            </p>
            <div className={styles.albums}>
                {albumData
                    .sort((a, b) => b.rating - a.rating)
                    .map((album) => (
                        <AlbumCard
                            key={album.id}
                            imageSource={album.imageSource}
                            title={album.title}
                            date={album.date}
                            rating={album.rating}
                        />
                    ))}
            </div>
            <div className={styles.songList}>
                <h2>Popular Songs</h2>
                {songData
                    .sort((a, b) => b.rating - a.rating)
                    .map((song) => (
                        <SongListItem {...song} />
                    ))}
            </div>
        </div>
    );
};

export default ArtistPage;
