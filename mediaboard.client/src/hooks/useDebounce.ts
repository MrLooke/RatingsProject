import { useState, useEffect } from "react";

export default function useDebounce<T>(value: T, delay: number = 500): T {
	const [debouncedVal, setDebouncedVal] = useState<T>(value);

	useEffect(() => {
		const timer = setTimeout(() => {
			setDebouncedVal(value);
		}, delay);

		return () => {
			clearTimeout(timer);
		};
	}, [value, delay]);

	return debouncedVal;
}
