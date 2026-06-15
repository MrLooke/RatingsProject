import { createFileRoute } from "@tanstack/react-router";
import ArtistPage from "@/features/artist/ArtistPage";

export const Route = createFileRoute("/artist")({
	component: RouteComponent,
});

function RouteComponent() {
	return <ArtistPage artistId={6512329} />;
}
