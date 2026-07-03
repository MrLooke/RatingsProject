import { useState } from "react";
import Star from "@/assets/star.svg?react";
import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Cards/Card";
import DefaultCover from "@/assets/music-album.svg?react";
import { useAuth } from "@/context/AuthContext";
import RatingDialog from "./RatingDialog";

const FullAlbumCard = ({
    albumId,
    title,
    year,
    imageSource,
    rating,
}: {
    albumId: number;
    title: string;
    year: string | undefined;
    imageSource?: string;
    rating?: number;
}) => {
    const { user } = useAuth();
    const [dialogOpen, setDialogOpen] = useState(false);

    const imageElement = imageSource ? (
        <img src={imageSource} />
    ) : (
        <DefaultCover stroke="currentColor" className={styles.defaultCover} />
    );

    return (
        <>
            <Card className={styles.albumCard}>
                <div className={styles.imageContainer}>{imageElement}</div>
                <div className={styles.info}>
                    <div className={styles.headers}>
                        <h2>{title}</h2>
                        {year && <p>{year}</p>}
                    </div>
                    <div className={styles.albumFooter}>
                        <div className={styles.rating}>{rating ?? "--"}</div>
                        {user && (
                            <button
                                className={styles.rateButton}
                                onClick={() => setDialogOpen(true)}
                            >
                                <Star className={styles.rateButtonIcon} />
                                Rate
                            </button>
                        )}
                    </div>
                </div>
            </Card>
            {dialogOpen && (
                <RatingDialog
                    albumId={albumId}
                    albumTitle={title}
                    onClose={() => setDialogOpen(false)}
                />
            )}
        </>
    );
};

export default FullAlbumCard;
