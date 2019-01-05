using Library.Configurations;
using Library.Playlists;
using Library.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        readonly IPlaylist _playlist;

        public MainController(IPlaylist playlist)
        {
            _playlist = playlist;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("api/playlist")]
        [HttpGet]
        public async Task<IEnumerable<Song>> Playlist()
        {            
           return await _playlist.GetSongsAsync().ConfigureAwait(false);
        }
    }
}