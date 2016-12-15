using System.Collections.Generic;
using Cinemation.Core.Util.Parser;

namespace Cinemation.Core.Util
{
    /// <summary>
    ///     Use seperate torrent parts to make a "normal" torrent title for sites that don't provide it (ex; yts.ag).
    ///     An example: "Spectre.2015.1080p.BluRay.H264.AAC-RARBG"
    ///     Order of items, which must be seperated with dots (.).
    ///     
    ///     1. Movie title
    ///     2. Movie year
    ///     3. Video resolution (480p, 720p, 1080p..)
    ///     4. Video source (BluRay, DVDRIP..)
    ///     5. Audio Channel (2.0, 5.1..)
    ///     6. Audio Codec (AAC, FLAC)
    ///     7. Video encoding (H264, X264..)
    ///     8. Group (-CMT..)
    /// </summary>
    public class TorrentTitle
    {

        public string MovieTitle { get; set; }

        private string Title { get; set; }

        public string MovieYear { get; set; }

        public bool Is3D = false;

        public string VideoResolution { get; set; }

        public string VideoSource { get; set; }

        public string AudioChannel { get; set; }

        public string AudioCodec { get; set; }

        public string VideoEncoding { get; set; }

        public string Group { get; set; }

        public string GetTitle()
        {
            var titleParts = new List<string>
            {
                MovieTitle.Replace(":", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(' ', '.')
            };

            if (MovieYear != null)
                titleParts.Add(MovieYear);

            if (!string.IsNullOrEmpty(VideoResolution))
                titleParts.Add(VideoResolution);

            if (!string.IsNullOrEmpty(VideoSource))
                titleParts.Add(VideoSource);

            if (!string.IsNullOrEmpty(AudioChannel))
                titleParts.Add(AudioChannel);

            if (!string.IsNullOrEmpty(AudioCodec))
                titleParts.Add(AudioCodec);

            if (!string.IsNullOrEmpty(VideoEncoding))
                titleParts.Add(VideoEncoding);

            if (Is3D)
                titleParts.Add("3D");

            if (!string.IsNullOrEmpty(Group))
                titleParts[titleParts.Count - 1] += $"-{Group}";

            return string.Join(".", titleParts);
        }
        
        public TorrentTitle(string Title, string SearchTitle)
        {
            SearchTitle = SearchTitle.Replace("+", ".");
            string NormalizedTitle = Title.Replace(" ", ".");
            string NormalizedSearchTitle = SearchTitle.Replace(" ", ".");
            if (!NormalizedTitle.Contains(NormalizedSearchTitle))
            {
                this.MovieTitle = "ERROR"; // TODO: Improved error handling
                return;
            }

            this.MovieTitle = SearchTitle;
            string TorrentInfo = NormalizedTitle.Replace(NormalizedSearchTitle, "");
            this.Is3D = TitleParser.Is3D(TorrentInfo);
            this.MovieYear = TitleParser.GetYear(TorrentInfo);
        }

    }
}
