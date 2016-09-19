using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    // TODO: Support imdb id.
    public class RarBgIndexer : TorrentIndexer
    {
        private const string Endpoint = "https://torrentapi.org/pubapi_v2.php";

        public RarBgIndexer() : base("RarBg")
        {
        }

        private string Token { get; set; }

        private DateTime TokenExpiry { get; set; }

        private bool TokenIsExpired => string.IsNullOrEmpty(Token) || TokenExpiry < DateTime.UtcNow;

        private async Task RefreshTokenAsync()
        {
            var response = await HttpClient.GetStringAsync($"{Endpoint}?get_token=get_token");
            var jToken = JsonConvert.DeserializeObject<JToken>(response);

            var token = jToken.Value<string>("token");

            if (string.IsNullOrEmpty(token))
                throw new Exception("Received an invalid token.");

            Token = token;
            TokenExpiry = DateTime.UtcNow.AddMinutes(14);
        }

        protected override async Task<List<TorrentData>> PerformSearchAsync(SearchData searchData)
        {
            if (TokenIsExpired)
                await RefreshTokenAsync();

            var torrentsData = new List<TorrentData>();
            var response = await HttpClient.GetStringAsync($"{Endpoint}?mode=search&category=movies&format=json_extended&search_string={searchData.MovieName}&token={Token}");

            var jToken = JsonConvert.DeserializeObject<JToken>(response);
            var torrents = jToken.Value<JArray>("torrent_results");

            if (torrents == null) return torrentsData;

            torrentsData.AddRange(
                torrents.Select(torrent => new TorrentData
                    {
                        Title = torrent.Value<string>("title"),
                        Magnet = torrent.Value<string>("download"),
                        Seeds = torrent.Value<int>("seeders"),
                        Size = torrent.Value<long>("size")
                    }
                )
            );

            return torrentsData;
        }
    }
}
