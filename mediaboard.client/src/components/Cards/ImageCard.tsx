import styles from "@/components/components.module.css";
import ImageWithDefault from "../ImageWithDefault";
import DefaultAlbumCover from "@/assets/music-album.svg?react";

interface ImageCardProps extends React.HTMLAttributes<HTMLDivElement> {
	header: string;
	subheader: string;
	imageSource?: string;
	children?: React.ReactNode;
	imageContainerClass?: string;
}

const ImageCard = ({
	header: title,
	subheader: artist,
	imageSource,
	children,
	imageContainerClass,
	className = "",
	...rest
}: ImageCardProps) => {
	return (
		<div className={`${styles.imageCard} ${className}`} {...rest}>
			<ImageWithDefault
				src={imageSource}
				alt={title + " Cover Image"}
				containerClass={imageContainerClass}
			>
				<DefaultAlbumCover />
			</ImageWithDefault>
			<div className={styles.footer}>
				<h2>{title}</h2>
				<h3>{artist}</h3>
			</div>
			{children}
		</div>
	);
};

export default ImageCard;
