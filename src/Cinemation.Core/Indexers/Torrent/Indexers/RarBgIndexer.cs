using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    public class RarBgIndexer : TorrentIndexer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override async Task<List<TorrentData>> Search(string query)
        {
            Logger.Debug("Running RarBg indexer search.");

            return new List<TorrentData>();
        }
    }
}
