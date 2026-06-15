import { createFileRoute } from "@tanstack/react-router";
import ArtistPage from "@/features/artist/ArtistPage";

export const Route = createFileRoute("/artist/$artistId")({
	component: RouteComponent,
});

function RouteComponent() {
	const { artistId } = Route.useParams();
	return <ArtistPage artistId={parseInt(artistId)} />;
}
