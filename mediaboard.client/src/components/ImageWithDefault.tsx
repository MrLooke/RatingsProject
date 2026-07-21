import DefaultCover from "@/assets/music-album.svg?react";
import styles from "@/components/components.module.css";
import { useState } from "react";

interface ImageDefaultProps extends React.HTMLAttributes<HTMLDivElement> {
	src?: string | null;
	alt?: string;
	children: React.ReactNode;
	containerClass?: string;
}

const ImageWithDefault = ({ src, alt, containerClass }: ImageDefaultProps) => {
	const [error, setError] = useState(false);

	if (!src || error) {
		return (
			<div
				className={`${styles.defaultImageContainer} ${containerClass}`}
			>
				<DefaultCover stroke="currentColor" />
			</div>
		);
	}

	return (
		<div className={`${styles.defaultImageContainer} ${containerClass}`}>
			<img src={src} alt={alt} onError={() => setError(true)} />
		</div>
	);
};

export default ImageWithDefault;
