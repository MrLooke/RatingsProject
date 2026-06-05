import type { ReactNode } from "react";
import styles from "@/features/artist/artist.module.css";

type ArtistHeaderProps = {
    children: ReactNode;
};

const ArtistHeader = ({ children }: ArtistHeaderProps) => {
    return (
        <div className={styles.artistName}>
            <div>{children}</div>
        </div>
    );
};

export default ArtistHeader;
