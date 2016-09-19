using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;

namespace Cinemation.Core.Indexers.Torrent
{
    public abstract class TorrentIndexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        protected TorrentIndexer(string name, bool enabled = true)
        {
            Name = name;
            Enabled = enabled;
            HttpClient = new HttpClient();
        }

        public string Name { get; }

        public bool Enabled { get; }

        public HttpClient HttpClient;

        public Task<List<TorrentData>> SearchByMovieTitle(string query)
        {
            return SearchAsync(new SearchData
            {
                MovieName = WebUtility.UrlEncode(query)
            });
        }

        private async Task<List<TorrentData>> SearchAsync(SearchData searchData)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"[{Name}]: Searching '{searchData.MovieName}'.");

            var stopwatch = Stopwatch.StartNew();
            var result = new List<TorrentData>(0);

            try
            {
                result = await PerformSearchAsync(searchData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[{Name}]: Threw an exception while performing a search.");
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"[{Name}]: Found '{result.Count}' torrents in {stopwatch.ElapsedMilliseconds}ms.");

            return result;
        }

        protected abstract Task<List<TorrentData>> PerformSearchAsync(SearchData searchData);
    }
}
