using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cinemation.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    // TODO: Support imdb id.
    public class YtsAgIndexer : TorrentIndexer
    {
        private const string EndpointMovies = "https://yts.ag/api/v2/list_movies.json";

        public YtsAgIndexer() : base("YtsAg")
        {
        }
        
        protected override async Task<List<TorrentData>> PerformSearchAsync(SearchData searchData)
        {
            var torrentsData = new List<TorrentData>();
            var response = await HttpClient.GetStringAsync($"{EndpointMovies}?query_term={searchData.MovieName}");

            var jToken = JsonConvert.DeserializeObject<JToken>(response);
            var movies = jToken.Value<JToken>("data")?.Value<JArray>("movies");

            if (movies == null) return torrentsData;

            foreach (var movie in movies)
            {
                var torrents = movie.Value<JToken>("torrents");
                if (torrents == null) continue;

                torrentsData.AddRange(
                    torrents.Select(torrent => new TorrentData
                        {
                            Title = new TorrentTitle
                            {
                                MovieTitle = movie.Value<string>("title"),
                                MovieYear = movie.Value<int>("year"),
                                VideoResolution = torrent.Value<string>("quality"),
                                VideoSource = "BluRay",
                                AudioChannel = "2.0",
                                AudioCodec = "AAC",
                                Group = "YTS"
                            }.GetTitle(),
                            Magnet = GetMagnetUri(torrent.Value<string>("hash"), movie.Value<string>("title_long")),
                            Seeds = torrent.Value<int>("seeds"),
                            Peers = torrent.Value<int>("peers"),
                            Size = torrent.Value<long>("size_bytes")
                        }
                    )
                );
            }

            return torrentsData;
        }

        private static string GetMagnetUri(string hash, string displayName)
        {
            if(string.IsNullOrEmpty(hash))
                throw new NullReferenceException("hash can't be null.");

            if(string.IsNullOrEmpty(displayName))
                throw new NullReferenceException("displayName can't be null.");

            return $"magnet:?xt=urn:btih:{hash}&dn={WebUtility.UrlEncode(displayName)}&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969%2Fannounce&tr=udp%3A%2F%2F9.rarbg.com%3A2750%2Fannounce&tr=udp%3A%2F%2Fp4p.arenabg.com%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.internetwarriors.net%3A1337&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337%2Fannounce";
        }
    }
}
