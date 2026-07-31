namespace MediaBoard.Server.Features.Album
{
    public interface IAlbumService
    {
        Task<AlbumInfoResult?> GetFullAlbumDataAsync(int albumId);
    }
}
