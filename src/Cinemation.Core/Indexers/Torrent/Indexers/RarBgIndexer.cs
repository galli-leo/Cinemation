using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemation.Core.Indexers.Torrent.Indexers
{
    public class RarBgIndexer : TorrentIndexer
    {
        public RarBgIndexer() : base(false)
        {
        }

        protected override async Task<List<TorrentData>> PerformSearchAsync(SearchData searchData)
        {
            return new List<TorrentData>();
        }
    }
}
