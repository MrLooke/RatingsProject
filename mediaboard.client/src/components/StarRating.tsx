import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";
import StarFull from "@/assets/star-full.svg?react";

const STAR_COUNT = 5;

interface StarRatingProps {
	rating: number;
	className?: string;
}

const StarRating = ({ rating, className }: StarRatingProps) => {
	const starValue = rating / 2;

	const getFillClass = (star: number) => {
		if (starValue >= star) return styles.starRatingFull;
		if (starValue >= star - 0.5) return styles.starRatingHalf;
		return styles.starRatingNone;
	};

	return (
		<div
			className={`${styles.starRating} ${className ?? ""}`}
			role="img"
			aria-label={`Rated ${rating} out of 10`}
		>
			{Array.from({ length: STAR_COUNT }, (_, i) => i + 1).map((star) => (
				<div key={star} className={styles.starRatingStar} aria-hidden>
					<Star className={styles.starRatingEmpty} />
					<div
						className={`${styles.starRatingFill} ${getFillClass(star)}`}
					>
						<StarFull className={styles.starRatingFilled} />
					</div>
				</div>
			))}
		</div>
	);
};

export default StarRating;
