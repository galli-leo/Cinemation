using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cinemation.Core.Indexers.Torrent;
using NLog;

namespace Cinemation.Core.Indexers
{
    public class Indexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<TorrentIndexer> TorrentIndexers;

        static Indexer()
        {
            TorrentIndexers = new List<TorrentIndexer>();

            foreach (var type in typeof(Indexer).GetTypeInfo().Assembly.GetTypes())
            {
                if (!type.GetTypeInfo().IsSubclassOf(typeof(TorrentIndexer))) continue;

                var torrentIndexer = (TorrentIndexer)Activator.CreateInstance(type);

                if (torrentIndexer.Enabled)
                    TorrentIndexers.Add(torrentIndexer);
            }
        }

        public static async Task SearchTorrents(string query)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"Searching torrents with the query: '{query}'.");

            var searchTasks = TorrentIndexers
                .Select(torrentIndexer => torrentIndexer.SearchByMovieTitle(query))
                .ToArray();

            await Task.WhenAll(searchTasks);

            foreach (var torrents in searchTasks.Select(x => x.Result))
            {
                foreach (var torrent in torrents)
                {
                    Logger.Debug(torrent.Title);
                }
            }
        }
    }
}
