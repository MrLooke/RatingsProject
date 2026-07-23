import { useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { useNavigate } from "@tanstack/react-router";
import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Cards/Card";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";
import RatingDialog from "./RatingDialog";
import RatingBadge from "@/components/RatingBadge";
import NoRatingBadge from "@/components/NoRatingBadge";
import StarRating from "@/components/StarRating";

const FullAlbumCard = ({
	albumId,
	title,
	year,
	format,
	imageSource,
	rating,
	userRating,
}: {
	albumId: number;
	title: string;
	year: string | undefined;
	format?: string | null;
	imageSource?: string;
	rating?: number;
	userRating?: number;
}) => {
	const { user } = useAuth();
	const navigate = useNavigate();
	const [dialogOpen, setDialogOpen] = useState(false);

	return (
		<>
			<Card className={styles.albumCard}>
				<ImageWithDefault
					containerClass={styles.imageContainer}
					src={imageSource}
					alt={title + "Cover Image/Art"}
				>
					<DefaultAlbumCover />
				</ImageWithDefault>

				<div className={styles.info}>
					<div className={styles.headers}>
						<h2 aria-label={title} title={title}>
							{title}
						</h2>
						{year && (
							<p>
								{year} · {format ?? "Misc"}
							</p>
						)}
					</div>
					<div className={styles.albumFooter}>
						{/* User's own rating will go here */}
						{userRating ? (
							<StarRating rating={userRating} />
						) : (
							<div className={styles.rating}>No rating yet</div>
						)}

						{rating ? (
							<RatingBadge
								rating={rating}
								className={styles.ratingButton}
								onClick={() => {
									if (user) setDialogOpen(true);
									else navigate({ to: "/login" });
								}}
							/>
						) : (
							<NoRatingBadge
								onClick={() => {
									if (user) setDialogOpen(true);
									else navigate({ to: "/login" });
								}}
							/>
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
