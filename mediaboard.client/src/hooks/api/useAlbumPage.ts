import { useQuery } from "@tanstack/react-query";
import { getAlbumPage } from "@/api/albumApi";

const useAlbumPage = (albumId: number) => {
	const { isPending, isError, data, error } = useQuery({
		queryKey: ["albums", "page", albumId],
		queryFn: () => getAlbumPage(albumId),
		// staleTime: 1000 * 60 * 20,
		// gcTime: 1000 * 60 * 10,
	});

	return { isPending, isError, data, error };
};

export default useAlbumPage;
