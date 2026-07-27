namespace MediaBoard.Server.Features.Songs
{
    public interface ISongService
    {
        Task<List<ReleaseSongResult>> GetSongsInAlbumAsync(int albumId);
    }
}
