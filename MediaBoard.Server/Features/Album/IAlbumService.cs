using MediaBoard.Server.Features.Songs;

namespace MediaBoard.Server.Features.Album
{
    public interface IAlbumService
    {
        Task<AlbumInfoResult?> GetFullAlbumDataAsync(int albumId);
    }
}
