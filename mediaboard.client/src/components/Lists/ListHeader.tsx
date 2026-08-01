import styles from "@/components/components.module.css";

interface TextOnlyChildrenProps {
	alignment: "left" | "center" | "right";
	children: string;
}

const ListHeader = ({ alignment, children }: TextOnlyChildrenProps) => {
	return (
		<h2 className={styles.listHeader} style={{ textAlign: alignment }}>
			{children}
		</h2>
	);
};

export default ListHeader;
