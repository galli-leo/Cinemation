namespace Cinemation.Parser.Elements
{
    /// <summary>
    /// Information which we can possibly extract from a file (torrent) name.
    /// </summary>
    public enum ElementCategory
    {
        AudioChannel,
        AudioCodec,
        AudioLanguage,
        FileName,
        MovieTitle,
        MovieYear,
        VideoCodec,
        VideoResolution,
        VideoSource,
        ReleaseGroup,
        Unknown
    }
}
