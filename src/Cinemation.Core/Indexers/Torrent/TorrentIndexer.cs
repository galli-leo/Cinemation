using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cinemation.Core.Indexers.Torrent
{
    public abstract class TorrentIndexer
    {
        public HttpClient HttpClient;

        protected TorrentIndexer()
        {
            HttpClient = new HttpClient();
        }

        public abstract Task<List<TorrentData>> Search(string query);
    }
}
