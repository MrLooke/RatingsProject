import { useEffect, useRef } from "react";
import useArtistSearch from "@/hooks/api/useArtistSearch";
import styles from "@/features/search/search.module.css";
import { Link } from "@tanstack/react-router";

const SearchItem = ({
	id,
	title,
	subtitle = null,
	imgSrc = null,
}: {
	id: number;
	title: string;
	subtitle?: string | null;
	imgSrc?: string | null;
}) => {
	const link = `/artist/${id}`;
	return (
		<Link to={link}>
			<li className={styles.searchItem}>
				<h2 title={title}>{title}</h2>
				{subtitle && <h3></h3>}
				{imgSrc && <img></img>}
			</li>
		</Link>
	);
};

const SearchResults = ({ query }: { query: string }) => {
	const { data, isFetching, fetchNextPage, hasNextPage } =
		useArtistSearch(query);
	const results = data?.pages.flat() ?? [];

	const containerRef = useRef<HTMLUListElement>(null);
	const sentinelRef = useRef<HTMLLIElement>(null);

	useEffect(() => {
		const sentinel = sentinelRef.current;
		const container = containerRef.current;
		if (!sentinel || !container) return;

		const observer = new IntersectionObserver(
			(entries) => {
				if (entries[0].isIntersecting && hasNextPage && !isFetching) {
					fetchNextPage();
				}
			},
			{ root: container, threshold: 0 },
		);

		observer.observe(sentinel);
		return () => observer.disconnect();
	}, [hasNextPage, isFetching, fetchNextPage]);

	if (!data && isFetching) {
		return (
			<ul
				className={styles.resultsContainer}
				onMouseDown={(e) => e.preventDefault()}
			>
				<li className={styles.searchStatusItem}>Loading...</li>
			</ul>
		);
	}

	if (results.length === 0) {
		return (
			<ul
				className={styles.resultsContainer}
				onMouseDown={(e) => e.preventDefault()}
			>
				<li className={styles.searchStatusItem}>No results</li>
			</ul>
		);
	}

	return (
		<ul
			ref={containerRef}
			className={styles.resultsContainer}
			onMouseDown={(e) => e.preventDefault()}
		>
			{results.map((artist) => (
				<SearchItem
					key={artist.id}
					id={artist.id}
					title={artist.name}
				/>
			))}
			{isFetching && (
				<li className={styles.searchStatusItem}>Loading...</li>
			)}
			<li ref={sentinelRef} />
		</ul>
	);
};

export default SearchResults;
