import { useState } from "react";
import useDebounce from "@/hooks/useDebounce";
import InputText from "@/components/InputText";
import SearchResults from "./SearchResults";
import styles from "@/features/search/search.module.css";

const SearchBar = ({ className = "" }: { className?: string }) => {
	const [query, setQuery] = useState("");
	const [isFocused, setIsFocused] = useState(false);
	const debouncedQuery = useDebounce(query, 300);

	const handleQuerySubmit = (queryString: string) => {
		setQuery(queryString);
	};

	return (
		<div className={`${styles.searchContainer} ${className}`}>
			<InputText
				className={styles.searchInput}
				onEnterPress={handleQuerySubmit}
				onFocus={() => setIsFocused(true)}
				onBlur={() => setIsFocused(false)}
				placeholder="Search..."
			/>

			{isFocused && debouncedQuery.length > 0 && (
				<SearchResults query={debouncedQuery} />
			)}
		</div>
	);
};

export default SearchBar;
