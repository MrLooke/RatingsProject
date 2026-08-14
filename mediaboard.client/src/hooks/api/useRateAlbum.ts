import { useMutation, useQueryClient } from "@tanstack/react-query";
import { submitRating } from "@/api/ratingApi";
import type { ArtistPage } from "@/api/artistsApi";

interface RateAlbumInput {
	albumId: number;
	score: number;
	review?: string;
}

interface RateAlbumContext {
	previous?: ArtistPage;
}

const useRateAlbum = (artistId: number) => {
	const queryClient = useQueryClient();
	const queryKey = ["artists", "page", artistId];

	return useMutation<void, Error, RateAlbumInput, RateAlbumContext>({
		mutationFn: ({ albumId, score, review }) =>
			submitRating(albumId, score, review),
		onMutate: async ({ albumId, score }) => {
			await queryClient.cancelQueries({ queryKey });
			const previous = queryClient.getQueryData<ArtistPage>(queryKey);

			if (previous) {
				queryClient.setQueryData<ArtistPage>(queryKey, {
					...previous,
					albums: previous.albums.map((album) =>
						album.id === albumId
							? { ...album, userRating: score }
							: album,
					),
				});
			}

			return { previous };
		},
		onError: (_err, _vars, context) => {
			if (context?.previous) {
				queryClient.setQueryData(queryKey, context.previous);
			}
		},
		onSettled: () => {
			queryClient.invalidateQueries({ queryKey });
		},
	});
};

export default useRateAlbum;
