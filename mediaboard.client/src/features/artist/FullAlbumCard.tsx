import { useState } from "react";
import Star from "@/assets/star.svg?react";
import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Cards/Card";
import ImageWithDefault from "@/components/ImageWithDefault";
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

	return (
		<>
			<Card className={styles.albumCard}>
				<ImageWithDefault
					containerClass={styles.imageContainer}
					src={imageSource}
					alt={title + "Cover Image/Art"}
				/>

				<div className={styles.info}>
					<div className={styles.headers}>
						<h2>{title}</h2>
						{year && <p>{year} · Album</p>}
					</div>
					<div className={styles.albumFooter}>
						<div className={styles.rating}>
							{rating ?? "No rating yet"}
						</div>
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
