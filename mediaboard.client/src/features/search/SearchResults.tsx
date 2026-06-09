import type { ArtistSearchDTO } from "@/api/artistsApi";
import type { InfiniteData } from "@tanstack/react-query";
import useArtistSearch from "@/hooks/api/useArtistSearch";
import styles from "@/features/search/search.module.css";
import React from "react";

const SearchItem = ({
	title,
	subtitle = null,
	imgSrc = null,
}: {
	title: string;
	subtitle?: string | null;
	imgSrc?: string | null;
}) => {
	return (
		<li className={styles.searchItem}>
			<h2 title={title}>{title}</h2>
			{subtitle && <h3></h3>}
			{imgSrc && <img></img>}
		</li>
	);
};

interface SearchResultsStructure {
	data: InfiniteData<ArtistSearchDTO[], unknown> | undefined;
}

const SearchResults = ({ query }: { query: string }) => {
	const { data }: SearchResultsStructure = useArtistSearch(query);

	return (
		<ul className={styles.resultsContainer}>
			{data?.pages.map((group, i) => (
				<React.Fragment key={i}>
					{group.map((artist) => (
						<SearchItem title={artist.name} />
					))}
				</React.Fragment>
			))}
		</ul>
	);
};

export default SearchResults;
