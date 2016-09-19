using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    public class YtsAgIndexer : TorrentIndexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string MoviesEndpoint = "https://yts.ag/api/v2/list_movies.json";

        public override async Task<List<TorrentData>> Search(string query)
        {
            Logger.Debug("Running YtsAg indexer search.");

            var torrentsData = new List<TorrentData>();
            var rawBody = await HttpClient.GetStringAsync($"{MoviesEndpoint}?query_term={query}");

            var jobject = JsonConvert.DeserializeObject<JObject>("{\"hoi\":true}");
            var movies = jobject.Value<JObject>("data")?.Value<JArray>("movies");

            return torrentsData;
        }
    }
}
