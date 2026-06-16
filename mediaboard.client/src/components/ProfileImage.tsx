import styles from "@/components/components.module.css";
import { useState } from "react";
import Person from "@/assets/person.svg?react";

interface ProfileImageProps extends React.HTMLAttributes<HTMLDivElement> {
	src?: string | null;
	alt?: string;
	className?: string;
}

const ProfileImage = ({ src, alt, className }: ProfileImageProps) => {
	const [error, setError] = useState(false);

	if (error || !src) {
		return (
			<div className={`${styles.profileImageContainer} ${className}`}>
				<Person stroke="currentColor" />
			</div>
		);
	}

	return (
		<div className={`${styles.profileImageContainer} ${className}`}>
			<img src={src} alt={alt} onError={() => setError(true)} />
		</div>
	);
};

export default ProfileImage;
