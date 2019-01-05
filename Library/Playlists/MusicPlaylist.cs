using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Configurations;
using Library.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Flurl;
using Flurl.Http;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using System.Text.RegularExpressions;
using Library.Exceptions;

namespace Library.Playlists
{
    public class MusicPlaylist : IPlaylist
    {
        readonly Kumiko _kumiko;
        readonly ILogger<MusicPlaylist> _logger;

        public int Category() => 0;

        public MusicPlaylist(IOptions<Kumiko> options,
            ILoggerFactory loggerFactory)
        {
            _kumiko = options.Value;
            _logger = loggerFactory.CreateLogger<MusicPlaylist>();
        }

        public async Task<IEnumerable<Song>> GetSongsAsync()
        {
            var content = await GetStringFromUrlAsync(_kumiko.Playlist);

            if (content == null)
                throw new AppException($"Loading playlist from {_kumiko.Playlist} failed.");

            using (var input = new StringReader(content))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();

                try
                {
                    var parser = new Parser(input);
                    var songs = deserializer.Deserialize<List<Song>>(parser);

                    foreach (var s in songs)
                    {
                        s.Src = GetFixedSrc(_kumiko.Playlist, s.Src);
                    }

                    return songs.OrderBy(x => x.Src);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deserializing yaml with songs from url {_kumiko.Playlist}.");
                    throw new AppException($"Parsing playlist from {_kumiko.Playlist} failed.");
                }
            }
        }

        async Task<string> GetStringFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new System.ArgumentException("Url must be set.", nameof(url));

            try
            {
                var content = await url.GetStringAsync().ConfigureAwait(false);
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading playlist from url {url}.");
            }

            return null;
        }

        string GetFixedSrc(string url, string src)
        {
            if (Regex.IsMatch(src, @"^https?:\/\/", RegexOptions.IgnoreCase))
                return src;

            Uri playlistUri;

            if (Uri.TryCreate(url, UriKind.Absolute, out playlistUri))
            {
                if (playlistUri.Scheme == Uri.UriSchemeHttp ||
                    playlistUri.Scheme == Uri.UriSchemeHttps)
                {
                    var segments = playlistUri.Segments;
                    var sn = segments.Count();

                    if (sn > 0)
                    {
                        var finalSegments = segments.Take(sn - 1);

                        var u = url
                            .ResetToRoot()
                            .AppendPathSegments(finalSegments)
                            .AppendPathSegment(src);

                        return u;
                    }
                }
            }

            return src;
        }
    }
}
