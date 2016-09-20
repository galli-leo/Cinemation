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

        protected TorrentIndexer(string name, bool enabled = true)
        {
            IndexerName = name;
            Enabled = enabled;
            HttpClient = new HttpClient();
            Configuration = new IndexerConfigurator();
            IndexerMetaData = new IndexerMetaData
            {
                Name = name
            };

            Initialize();
        }

        public string IndexerName { get; }

        public bool Enabled { get; }

        protected HttpClient HttpClient { get; }

        protected IndexerConfigurator Configuration { get; }

        private IndexerMetaData IndexerMetaData { get; }

        /// <summary>
        ///     Should only be called from the constructor once.
        /// </summary>
        private void Initialize()
        {
            Configure(Configuration);
        }

        protected abstract void Configure(IndexerConfigurator config);

        public async Task<List<TorrentData>> SearchAsync(SearchData searchData)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"[{IndexerName}]: Searching '{searchData.MovieName}'.");

            var stopwatch = Stopwatch.StartNew();
            var result = new List<TorrentData>(0);

            try
            {
                result = await PerformSearchAsync(searchData);
                result.ForEach(x => x.IndexerMetaData = IndexerMetaData);
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
