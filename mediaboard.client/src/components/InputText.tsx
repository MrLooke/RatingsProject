import styles from "@/components/components.module.css";

interface InputTextProps extends React.InputHTMLAttributes<HTMLInputElement> {
	onEnterPress?: (inputValue: string) => void;
}

const InputText = ({
	className = "",
	onEnterPress = () => {},
	...props
}: InputTextProps) => {
	const onSubmit = (event: React.KeyboardEvent<HTMLInputElement>) => {
		if (event.key == "Enter") {
			onEnterPress(event.currentTarget.value);
		}
	};

	const combinedClasses = `${styles.baseTextInput} ${className}`;
	return (
		<input
			className={combinedClasses}
			onKeyDown={onSubmit}
			{...props}
		></input>
	);
};

export default InputText;
