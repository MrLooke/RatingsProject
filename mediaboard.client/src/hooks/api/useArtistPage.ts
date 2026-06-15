import { useQuery } from "@tanstack/react-query";
import { getArtistPage } from "@/api/artistsApi";

const useArtistPage = (id: number) => {
	const { isPending, isError, data, error } = useQuery({
		queryKey: ["artists", "page", id],
		queryFn: () => getArtistPage(id),
	});

	return { isPending, isError, data, error };
};

export default useArtistPage;
