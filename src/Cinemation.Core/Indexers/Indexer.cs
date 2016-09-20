using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public static Task SearchTorrents(string query)
        {
            return SearchTorrents(new SearchData
            {
                MovieName = query
            });
        }

        public static async Task SearchTorrents(SearchData searchData)
        {
            searchData.MovieName = WebUtility.UrlEncode(searchData.MovieName);

            if (Logger.IsDebugEnabled)
                Logger.Debug($"Searching torrents for the movie: '{searchData.MovieName}'.");

            var searchTasks = TorrentIndexers
                .Select(torrentIndexer => torrentIndexer.SearchAsync(searchData))
                .ToArray();

            await Task.WhenAll(searchTasks);

            var finalTorrents = new List<TorrentData>();

            // TODO: Filter out duplicates.. but how? (Maybe match torrent file names??)
            foreach (var torrents in searchTasks.Select(x => x.Result))
            {
                finalTorrents.AddRange(torrents);
            }

            foreach (var torrent in finalTorrents)
            {
                Logger.Debug($"[{torrent.IndexerMetaData.Name}]: {torrent.Title}");
            }
        }
    }
}
