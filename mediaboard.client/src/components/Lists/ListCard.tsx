import styles from "@/components/components.module.css";

interface ListCardProps extends React.HTMLAttributes<HTMLDivElement> {
	children: React.ReactNode;
}

const ListCard = ({ children, className = "", ...rest }: ListCardProps) => {
	const combinedCLasses = `${styles.listCard} ${className}`;

	return (
		<div className={combinedCLasses} {...rest}>
			{children}
		</div>
	);
};

export default ListCard;
