import styles from "@/components/components.module.css";

interface InputTextProps extends React.InputHTMLAttributes<HTMLInputElement> {}

const InputText = ({ className = "", ...props }: InputTextProps) => {
    const combinedClasses = `${styles.baseTextInput} ${className}`;
    return <input className={combinedClasses} {...props}></input>;
};

export default InputText;
