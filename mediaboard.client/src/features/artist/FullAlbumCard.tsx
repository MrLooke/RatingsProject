import { useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { useNavigate, Link } from "@tanstack/react-router";
import styles from "@/features/artist/artist.module.css";
import Card from "@/components/Cards/Card";
import ImageWithDefault from "@/components/ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";
import RatingDialog from "./RatingDialog";
import RatingBadge from "@/components/RatingBadge";
import NoRatingBadge from "@/components/NoRatingBadge";
import StarRating from "@/components/StarRating";
import useRateAlbum from "@/hooks/api/useRateAlbum";

const FullAlbumCard = ({
	artistId,
	albumId,
	title,
	year,
	format,
	imageSource,
	rating,
	userRating,
}: {
	artistId: number;
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
	const rateAlbum = useRateAlbum(artistId);

	const albumLink = `/album/${albumId}`;

	const handleRatingSubmit = (score: number, review?: string) =>
		rateAlbum.mutateAsync({ albumId, score, review });

	return (
		<>
			<Link to={albumLink} style={{ textDecoration: "none" }}>
				<Card className={styles.albumCard} hasHover>
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
							{userRating ? (
								<StarRating rating={userRating} />
							) : (
								<div className={styles.rating}>
									No rating yet
								</div>
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
			</Link>
			{dialogOpen && (
				<RatingDialog
					albumTitle={title}
					initialRating={userRating}
					onClose={() => setDialogOpen(false)}
					onSubmit={handleRatingSubmit}
				/>
			)}
		</>
	);
};

export default FullAlbumCard;
