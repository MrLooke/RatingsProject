import styles from "@/components/components.module.css";

interface ListItemProps extends React.HTMLAttributes<HTMLDivElement> {
	clickable?: boolean;
	children: React.ReactNode;
}

const ListItem = ({
	clickable = false,
	className = "",
	children,
	...rest
}: ListItemProps) => {
	const combinedCLasses = `${styles.listItem} ${className}`;

	return (
		<div
			className={`${combinedCLasses} ${clickable && styles.listItemClickable}`}
			{...rest}
		>
			{children}
		</div>
	);
};

export default ListItem;
