using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemation.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    public class YtsAgIndexer : TorrentIndexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string MoviesEndpoint = "https://yts.ag/api/v2/list_movies.json";

        protected override async Task<List<TorrentData>> PerformSearchAsync(SearchData searchData)
        {
            var torrentsData = new List<TorrentData>();
            var rawBody = await HttpClient.GetStringAsync($"{MoviesEndpoint}?query_term={searchData.MovieName}");

            var jobject = JsonConvert.DeserializeObject<JToken>(rawBody);
            var movies = jobject.Value<JToken>("data")?.Value<JArray>("movies");

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
                            Seeds = torrent.Value<int>("seeds"),
                            Peers = torrent.Value<int>("peers"),
                            Size = torrent.Value<long>("size_bytes")
                        }
                    )
                );
            }

            return torrentsData;
        }
    }
}
