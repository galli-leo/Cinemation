namespace Cinemation.Core.Indexers.Torrent
{
    public class TorrentData
    {

        /// <summary>
        ///     Torrent title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The torrent magnet uri.
        /// </summary>
        public string Magnet { get; set; }

        /// <summary>
        ///     Torrent seeders count.
        /// </summary>
        public int Seeds { get; set; }

        /// <summary>
        ///     Torrent peer count.
        /// </summary>
        public int Peers { get; set; }

        /// <summary>
        ///     Torrent size in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        ///     MetaData about the 
        /// </summary>
        public IndexerMetaData IndexerMetaData;

    }
}
