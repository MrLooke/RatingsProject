import { createFileRoute } from "@tanstack/react-router";
import AlbumPage from "@/features/album/AlbumPage";

export const Route = createFileRoute("/album/$albumId")({
	component: RouteComponent,
});

function RouteComponent() {
	const { albumId } = Route.useParams();
	return <AlbumPage albumId={parseInt(albumId)} />;
}
