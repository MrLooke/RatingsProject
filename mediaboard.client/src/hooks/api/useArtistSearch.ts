import { useInfiniteQuery } from "@tanstack/react-query";
import { searchArtists } from "@/api/artistsApi";

interface PageItemParams {
	lastId: number;
	lastRankScore: number;
}

const useArtistSearch = (query: string) => {
	const { data, error, fetchNextPage, hasNextPage } = useInfiniteQuery({
		queryKey: ["artists", "search", query],
		queryFn: () => searchArtists(query),
		enabled: query.length > 0,
		initialPageParam: { lastId: -1, lastRankScore: -1 },
		getNextPageParam: (lastPage) => {
			const last = lastPage[lastPage.length - 1];
			const nextPageParams: PageItemParams = {
				lastId: last.id,
				lastRankScore: last.rankScore,
			};

			return nextPageParams;
		},
	});

	return { data, error, fetchNextPage, hasNextPage };
};

export default useArtistSearch;
