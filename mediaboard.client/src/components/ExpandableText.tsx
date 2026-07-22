import { useState, useRef, useEffect } from "react";
import styles from "@/components/components.module.css";

const ExpandableText = ({
	children,
	lineClamp,
	onClick = () => {},
	className = "",
}: {
	children: React.ReactNode;
	lineClamp: number;
	onClick?: () => void;
	className?: string;
}) => {
	const [isOpen, setIsOpen] = useState(false);
	const textRef = useRef<HTMLDivElement | null>(null);
	const [isOverflow, setIsOverflow] = useState(false);

	useEffect(() => {
		const node = textRef.current;
		if (!node) return;

		setIsOverflow(
			node.scrollHeight > node.clientHeight ||
				node.scrollWidth > node.clientWidth,
		);
	}, [children]);

	let expandButton = <span></span>;
	if (isOverflow) {
		if (isOpen) {
			expandButton = (
				<button
					className={styles.expandButton}
					onClick={() => {
						setIsOpen(false);
						onClick();
					}}
				>
					Read Less {"^"}
				</button>
			);
		} else {
			expandButton = (
				<button
					className={styles.expandButton}
					onClick={() => {
						setIsOpen(true);
						onClick();
					}}
				>
					Read More {">"}
				</button>
			);
		}
	}

	return (
		<div
			ref={textRef}
			className={`${styles.expandableDiv} ${className} ${isOpen ? styles.expandedText : styles.clampedText}`}
			style={!isOpen ? { WebkitLineClamp: lineClamp } : undefined}
		>
			{children}
			{expandButton}
		</div>
	);
};

export default ExpandableText;
