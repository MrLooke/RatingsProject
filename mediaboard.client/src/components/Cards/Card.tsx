import styles from "@/components/components.module.css";

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
	hasHover?: boolean;
	children: React.ReactNode;
}

const Card = ({
	children,
	hasHover = false,
	className = "",
	...rest
}: CardProps) => {
	const combinedClasses = `${styles.baseCard} ${className} ${hasHover ? styles.baseWithHover : ""}`;
	return (
		<div className={combinedClasses} {...rest}>
			{children}
		</div>
	);
};

export default Card;
