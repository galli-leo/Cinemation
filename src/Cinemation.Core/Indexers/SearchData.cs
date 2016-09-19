namespace Cinemation.Core.Indexers
{
    public class SearchData
    {

        /// <summary>
        ///     The name of the movie.
        /// </summary>
        public string MovieName { get; set; }

        /// <summary>
        ///     The year the movie was made in.
        /// </summary>
        public int MovieYear { get; set; }

        /// <summary>
        ///     The IMDb code. 
        ///     Example: imdb.com/title/(ttxxxxxxx)/
        /// </summary>
        public string ImdbCode { get; set; }

    }
}
