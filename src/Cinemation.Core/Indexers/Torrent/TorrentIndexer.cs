using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;

namespace Cinemation.Core.Indexers.Torrent
{
    public abstract class TorrentIndexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        protected TorrentIndexer(bool enabled = true)
        {
            Enabled = enabled;
            HttpClient = new HttpClient();
        }

        public bool Enabled { get; }

        public HttpClient HttpClient;

        /// <summary>
        ///     Class name of the current indexer.
        /// </summary>
        private string IndexerName => GetType().Name;

        public Task<List<TorrentData>> SearchByMovieTitle(string query)
        {
            return SearchAsync(new SearchData
            {
                MovieName = query
            });
        }

        private async Task<List<TorrentData>> SearchAsync(SearchData searchData)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"[{IndexerName}]: Searching '{searchData.MovieName}'.");

            var stopwatch = Stopwatch.StartNew();
            var result = new List<TorrentData>(0);

            try
            {
                result = await PerformSearchAsync(searchData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"[{IndexerName}]: Threw an exception while performing a search.");
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"[{IndexerName}]: Found '{result.Count}' torrents in {stopwatch.ElapsedMilliseconds}ms.");

            return result;
        }

        protected abstract Task<List<TorrentData>> PerformSearchAsync(SearchData searchData);
    }
}
