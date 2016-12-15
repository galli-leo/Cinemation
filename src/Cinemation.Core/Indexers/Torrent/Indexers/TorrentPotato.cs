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
    public class TorrentPotatoIndexer : TorrentIndexer
    {
        private string EndpointMovies = "http://127.0.0.1:9117/potato/privatehd";
        private string APIKey = "vu5gdo460fvooqhlf58nqvjxmgaxma8v";

        public TorrentPotatoIndexer() : base("TorrentPotato")
        {
        }

        protected override void Configure(IndexerConfigurator config)
        {
            
        }

        protected override async Task<List<TorrentData>> PerformSearchAsync(SearchData searchData)
        {
            var queryTerm = searchData.HasImdbCode ? $"imdbid={ searchData.ImdbCode}" : $"search={searchData.MovieName}";

            var torrentsData = new List<TorrentData>();
            var response = await HttpClient.GetStringAsync($"{EndpointMovies}?passkey={APIKey}&{queryTerm}");

            var jToken = JsonConvert.DeserializeObject<JToken>(response);
            var movies = jToken.Value<JToken>("results");

            if (movies == null) return torrentsData;

            foreach (var movie in movies)
            {
                var downloadURL = movie.Value<JToken>("download_url");
                if (downloadURL == null) continue;

                torrentsData.Add( new TorrentData
                        {
                            Title = new TorrentTitle
                            {
                                MovieTitle = movie.Value<string>("release_name"),
                                MovieYear = 1999,
                                VideoResolution = "",
                                VideoSource = "BluRay",
                                AudioChannel = "2.0",
                                AudioCodec = "AAC",
                                Group = "YTS"
                            }.GetTitle(),
                            Magnet = downloadURL.ToString(),
                            Seeds = movie.Value<int>("seeders"),
                            Peers = movie.Value<int>("leechers"),
                            Size = movie.Value<long>("size"),
                        }
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
