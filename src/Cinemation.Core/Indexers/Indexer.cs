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
                if (type.GetTypeInfo().IsSubclassOf(typeof(TorrentIndexer)))
                {
                    TorrentIndexers.Add((TorrentIndexer) Activator.CreateInstance(type));
                }
            }
        }

        public static void SearchTorrents(string query)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug($"Searching torrents with the query: {query}");

            var searchTasks = TorrentIndexers
                .Select(torrentIndexer => torrentIndexer.Search(query))
                .Cast<Task>()
                .ToArray();

            Task.WaitAll(searchTasks);
        }
    }
}
