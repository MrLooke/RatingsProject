import styles from "@/components/components.module.css";

interface ListItemProps extends React.HTMLAttributes<HTMLDivElement> {
	clickable?: boolean;
	children: React.ReactNode;
}

const ListItem = ({
	clickable: hasHover = false,
	children,
	...rest
}: ListItemProps) => {
	return (
		<div
			className={`${styles.listItem} ${styles.listItemClickable}`}
			{...rest}
		>
			{children}
		</div>
	);
};

export default ListItem;
