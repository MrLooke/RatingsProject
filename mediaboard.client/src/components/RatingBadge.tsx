import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";

interface RatingBadgeProps {
	rating: number;
	onClick?: () => void;
}

const RatingBadge = ({ rating, onClick }: RatingBadgeProps) => {
	return (
		<div className={styles.ratingBadge} onClick={onClick}>
			<Star stroke="currentColor" />
			<p>{rating}</p>
		</div>
	);
};

export default RatingBadge;
