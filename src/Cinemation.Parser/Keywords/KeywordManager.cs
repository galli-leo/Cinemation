using System;
using System.Collections.Generic;
using Cinemation.Parser.Elements;
using Cinemation.Parser.Tokens;

namespace Cinemation.Parser.Keywords
{
    /// <summary>
    /// Used to easily identify tokens.
    /// All keywords will be transformed to upper-case.
    /// </summary>
    public static class KeywordManager
    {
        private static readonly Dictionary<string, ElementCategory> Keywords;

        static KeywordManager()
        {
            Keywords = new Dictionary<string, ElementCategory>();

            AddKeys(ElementCategory.AudioChannel, new []
            {
                "2.0", "2.0CH", "2CH", "5.1", "5.1CH", "DTS", "DTS-ES", "DTS5.1", "TRUEHD5.1"
            });

            AddKeys(ElementCategory.AudioCodec, new []
            {
                "AAC", "AACX2", "AACX3", "AACX4", "AC3", "FLAC", "FLACX2", "FLACX3", "FLACX4", "LOSSLESS", "MP3", "OGG",
                "VORBIS"
            });

            AddKeys(ElementCategory.VideoCodec, new []
            {
                "8BIT", "8-BIT", "10BIT", "10BITS", "10-BIT", "10-BITS", "HI10P", "H264", "H265", "H.264", "H.265",
                "X264", "X265", "X.264", "AVC", "HEVC", "DIVX", "DIVX5", "DIVX6", "XVID"
            });

            AddKeys(ElementCategory.VideoResolution, new []
            {
                "480P", "720P", "1080P"
            });

            AddKeys(ElementCategory.VideoSource, new[]
            {
                "BD", "BDRIP", "BRRIP", "BLURAY", "BLU-RAY", "DVD", "DVD5", "DVD9", "DVD-R2J", "DVDRIP", "DVD-RIP",
                "R2DVD", "R2J", "R2JDVD", "R2JDVDRIP", "HDTV", "HDTVRIP", "TVRIP", "TV-RIP", "WEBCAST", "WEBRIP",
                "HD-CAM"
            });
        }

        private static void AddKeys(ElementCategory category, IEnumerable<string> keywords)
        {
            foreach (var keyword in keywords)
            {
                if (string.IsNullOrEmpty(keyword))
                    continue;

                var keywordUpper = keyword.ToUpper();

                if (Keywords.ContainsKey(keywordUpper))
                    throw new Exception($"Keyword '{keywordUpper}' already exists.");

                Keywords.Add(keywordUpper, category);
            }
        }

        // TODO: Search leftover text?
        public static List<TokenRange> Peek(FileNameParser parser, TokenRange range)
        {
            var preidentifiedTokens = new List<TokenRange>();
            var tokenPart = range.Content.ToUpper();

            foreach (var keyword in Keywords)
            {
                var offsetPosition = tokenPart.IndexOf(keyword.Key, StringComparison.Ordinal);
                if (offsetPosition != -1)
                {
                    var startIndex = offsetPosition + range.StartIndex;
                    var tokenRange = new TokenRange(parser.FileName, startIndex, startIndex + keyword.Key.Length);
                    preidentifiedTokens.Add(tokenRange);
                    parser.Elements.Add(keyword.Value, tokenRange.Content);
                }
            }

            return preidentifiedTokens;
        }
    }
}
