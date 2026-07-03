import { useState } from "react";
import Star from "@/assets/star.svg?react";
import { submitRating } from "@/api/ratingApi";
import Button from "@/components/Button";
import styles from "./RatingDialog.module.css";

const StarPicker = ({
    value,
    onChange,
}: {
    value: number;
    onChange: (v: number) => void;
}) => {
    const [hovered, setHovered] = useState<number | null>(null);
    const display = hovered ?? value;

    const getFill = (i: number): "full" | "half" | "none" => {
        if (display >= i) return "full";
        if (display >= i - 0.5) return "half";
        return "none";
    };

    return (
        <div
            className={styles.starRow}
            onMouseLeave={() => setHovered(null)}
        >
            {[1, 2, 3, 4, 5].map((i) => {
                const fill = getFill(i);
                return (
                    <div key={i} className={styles.star}>
                        <div className={styles.starVisual} aria-hidden>
                            <Star className={styles.starEmpty} />
                            <div className={`${styles.starFill} ${styles[fill]}`}>
                                <Star className={styles.starFilled} />
                            </div>
                        </div>
                        <div
                            className={styles.hitboxLeft}
                            onMouseEnter={() => setHovered(i - 0.5)}
                            onClick={() => onChange(i - 0.5)}
                        />
                        <div
                            className={styles.hitboxRight}
                            onMouseEnter={() => setHovered(i)}
                            onClick={() => onChange(i)}
                        />
                    </div>
                );
            })}
        </div>
    );
};

const RatingDialog = ({
    albumTitle,
    albumId,
    onClose,
}: {
    albumTitle: string;
    albumId: number;
    onClose: () => void;
}) => {
    const [stars, setStars] = useState(0);
    const [review, setReview] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (stars === 0) {
            setError("Please select a rating.");
            return;
        }
        setError(null);
        setLoading(true);
        try {
            const score = stars * 2;
            await submitRating(albumId, score, review.trim() || undefined);
            onClose();
        } catch (err) {
            setError(
                err instanceof Error ? err.message : "Failed to submit rating.",
            );
        } finally {
            setLoading(false);
        }
    };

    const starLabel =
        stars > 0 ? `${stars} / 5 (${stars * 2} / 10)` : "Select a rating";

    return (
        <div className={styles.overlay} onClick={onClose}>
            <div
                className={styles.dialog}
                onClick={(e) => e.stopPropagation()}
            >
                <div className={styles.header}>
                    <h3>Rate — {albumTitle}</h3>
                    <button
                        className={styles.closeButton}
                        onClick={onClose}
                        aria-label="Close"
                    >
                        ✕
                    </button>
                </div>
                <form onSubmit={handleSubmit} className={styles.form}>
                    <StarPicker value={stars} onChange={setStars} />
                    <span className={styles.starLabel}>{starLabel}</span>
                    <textarea
                        className={styles.reviewInput}
                        value={review}
                        onChange={(e) => setReview(e.target.value)}
                        placeholder="Write a review... (optional)"
                        rows={4}
                        maxLength={1000}
                    />
                    {error && <p className={styles.error}>{error}</p>}
                    <div className={styles.actions}>
                        <Button
                            type="button"
                            variant="neutral"
                            onClick={onClose}
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={loading || stars === 0}
                        >
                            {loading ? "Submitting..." : "Submit"}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default RatingDialog;
