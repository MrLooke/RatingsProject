import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { searchArtists } from "@/api/artistsApi";
import InputText from "@/components/InputText";

const SearchBar = () => {
	const [query, setQuery] = useState("");

	const artists = useQuery({
		queryKey: ["search", query],
		queryFn: () => searchArtists(query),
		enabled: query.length > 0,
	});

	const handleQuerySubmit = (queryString: string) => {
		setQuery(queryString);
	};

	return (
		<InputText
			onEnterPress={handleQuerySubmit}
			style={{ width: "350px", marginLeft: "25px", marginRight: "5px" }}
			placeholder="Search..."
		/>
	);
};

export default SearchBar;
