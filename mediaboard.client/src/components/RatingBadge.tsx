import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";

interface RatingBadgeProps {
	rating: number;
	className?: string;
	onClick?: () => void;
}

const RatingBadge = ({ rating, className, onClick }: RatingBadgeProps) => {
	return (
		<div className={`${styles.ratingBadge} ${className}`} onClick={onClick}>
			<Star stroke="currentColor" />
			<p>{(rating / 2).toFixed(2)}</p>
		</div>
	);
};

export default RatingBadge;
