import styles from "@/components/components.module.css";
import Star from "@/assets/star.svg?react";

const NoRatingBadge = ({
	className,
	onClick,
}: {
	className?: string;
	onClick?: () => void;
}) => {
	return (
		<div
			className={`${styles.ratingBadge} ${styles.noRatingBadge} ${className}`}
			onClick={onClick}
		>
			<Star stroke="currentColor" />
			<p>Rate</p>
		</div>
	);
};

export default NoRatingBadge;
