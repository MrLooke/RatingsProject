import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";

const NoRatingBadge = ({
	className,
	onClick,
}: {
	className?: string;
	onClick?: (e: React.MouseEvent) => void;
}) => {
	return (
		<div
			className={`${styles.ratingBadge} ${styles.noRatingBadge} ${className}`}
			onClick={(e) => {
				e.preventDefault();
				// e.stopPropagation();
				onClick?.(e);
			}}
		>
			<Star stroke="currentColor" />
			<p>Rate</p>
		</div>
	);
};

export default NoRatingBadge;
