import styles from "@/components/components.module.css";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: "neutral" | "primary" | "secondary";
    size?: "small" | "medium" | "large";
}

const Button = ({
    variant = "primary",
    size = "medium",
    className = "",
    children,
    ...rest
}: ButtonProps) => {
    const variantClasses = {
        neutral: styles.neutralButton,
        primary: styles.primaryButton,
        secondary: styles.secondaryButton,
    };

    const sizeClasses = {
        small: styles.smallButton,
        medium: styles.mediumButton,
        large: styles.largeButton,
    };

    const combinedClasses = `${styles.baseButton} ${variantClasses[variant]} ${sizeClasses[size]} ${className}`;

    return (
        <button className={combinedClasses} {...rest}>
            {children}
        </button>
    );
};

export default Button;
