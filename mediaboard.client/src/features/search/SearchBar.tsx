import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { searchArtists } from "@/api/artistsApi";
import useDebounce from "@/hooks/useDebounce";
import InputText from "@/components/InputText";
import SearchResults from "./SearchResults";
import styles from "@/features/search/search.module.css";

const SearchBar = () => {
	const [query, setQuery] = useState("");
	const debouncedQuery = useDebounce(query, 300);

	const handleQuerySubmit = (queryString: string) => {
		setQuery(queryString);
	};

	return (
		<div className={styles.searchContainer}>
			<InputText
				className={styles.searchInput}
				onEnterPress={handleQuerySubmit}
				placeholder="Search..."
			/>

			<SearchResults query={debouncedQuery} />
		</div>
	);
};

export default SearchBar;
