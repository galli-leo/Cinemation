using System;

namespace Cinemation.Core.Indexers
{
    internal class RarBgIndexer : Indexer
    {
        private static readonly Uri BaseUri = new Uri("https://torrentapi.org/pubapi_v2.php");

        public override Uri GetBaseUri()
        {
            return BaseUri;
        }
    }
}
