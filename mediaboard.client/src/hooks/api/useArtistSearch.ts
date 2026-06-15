import { useInfiniteQuery } from "@tanstack/react-query";
import { searchArtists } from "@/api/artistsApi";

const useArtistSearch = (query: string) => {
	const { data, error, fetchNextPage, hasNextPage, isFetching } =
		useInfiniteQuery({
			queryKey: ["artists", "search", query],
			queryFn: ({ pageParam }) =>
				searchArtists(
					query,
					pageParam?.lastId ?? null,
					pageParam?.lastRankScore ?? null,
				),
			enabled: query.length > 0,
			initialPageParam: null as {
				lastId: number;
				lastRankScore: number;
			} | null,
			getNextPageParam: (lastPage) => {
				if (lastPage.length === 0) return undefined;
				const last = lastPage[lastPage.length - 1];
				return { lastId: last.id, lastRankScore: last.rankScore };
			},
		});

	return { data, error, fetchNextPage, hasNextPage, isFetching };
};

export default useArtistSearch;
