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

        
        public bool HasMagnet => !string.IsNullOrEmpty(Magnet);

        /// <summary>
        ///     The download url to torrent file.
        /// </summary>
        public string DownloadULR { get; set; }

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
        ///     The score of the torrent. The higher the score, the more likely the torrent will get chosen.
        ///     TODO: Determine parameters which decide score.
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        ///     MetaData about the 
        /// </summary>
        public IndexerMetaData IndexerMetaData;

    }
}
