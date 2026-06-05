import styles from "@/components/components.module.css";

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
    children: React.ReactNode;
}

const Card = ({ children, className = "", ...rest }: CardProps) => {
    const combinedClasses = `${styles.baseCard} ${className}`;
    return (
        <div className={combinedClasses} {...rest}>
            {children}
        </div>
    );
};

export default Card;
