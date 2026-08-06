import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";
interface RatingBadgeProps {
	rating: number;
	className?: string;
	onClick?: (e: React.MouseEvent) => void;
}

const RatingBadge = ({ rating, className, onClick }: RatingBadgeProps) => {
	return (
		<div
			className={`${styles.ratingBadge} ${className}`}
			onClick={(e) => {
				e.preventDefault();
				// e.stopPropagation();
				onClick?.(e);
			}}
		>
			<Star stroke="currentColor" />
			<p>{(rating / 2).toFixed(2)}</p>
		</div>
	);
};

export default RatingBadge;
