import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Card";

const AlbumCard = ({
    imageSource,
    title,
    date,
    rating,
}: {
    imageSource: string;
    title: string;
    date: string;
    rating: number;
}) => {
    return (
        <Card className={styles.albumCard}>
            <img src={imageSource} />
            <div className={styles.info}>
                <div className={styles.headers}>
                    <h2>{title}</h2>
                    <p>{date}</p>
                </div>

                <div className={styles.rating}>{rating}</div>
            </div>
        </Card>
    );
};

export default AlbumCard;
