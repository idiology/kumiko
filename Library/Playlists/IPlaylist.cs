using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Tokens;

namespace Library.Playlists
{
    public interface IPlaylist
    {
        int Category();
        Task<IEnumerable<Song>> GetSongsAsync();
    }
}