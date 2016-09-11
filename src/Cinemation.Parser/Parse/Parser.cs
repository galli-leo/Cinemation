using System.Linq;
using System.Collections.Generic;
using Cinemation.Parser.Elements;
using Cinemation.Parser.Tokens;

namespace Cinemation.Parser.Parse
{
    /// <summary>
    /// Helps identify more complicated <see cref="ElementCategory"/>s.
    /// </summary>
    internal class Parser
    {
        /// <summary>
        /// Minimum year of a movie.
        /// </summary>
        private const int MovieYearMin = 1900;

        /// <summary>
        /// Maximum year of a movie.
        /// </summary>
        private const int MovieYearMax = 2050;

        private readonly FileNameParser _fileNameParser;

        public Parser(FileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
        }

        public void Parse()
        {
            SearchForKeywords();
            SearchForNumbers();
            SearchForTitle();
        }

        private void IdentifiedToken(ElementCategory category, IReadOnlyCollection<Token> tokens)
        {
            var tokensContent = string.Join(" ", tokens.Select(x => x.Range.Content).ToArray());

            _fileNameParser.Elements.Add(category, tokensContent);

            foreach (var token in tokens)
                token.Category = TokenCategory.Identifier;
        }

        private void IdentifiedToken(ElementCategory category, Token token)
        {
            _fileNameParser.Elements.Add(category, token.Range.Content);
            token.Category = TokenCategory.Identifier;
        }

        private void SearchForKeywords()
        {
            foreach (var token in GetRemainingTokens())
            {
                var tokenNode = _fileNameParser.Tokens.Find(token);
                var category = ElementCategory.Unknown;

                // Identify
                

                // If identified
                if (category != ElementCategory.Unknown)
                {
                    IdentifiedToken(category, token);
                }
            }
        }

        private void SearchForNumbers()
        {
            foreach (var token in GetRemainingTokens())
            {
                if (!token.Range.Content.All(char.IsDigit))
                    continue;

                var number = int.Parse(token.Range.Content);

                // Movie year
                if (number >= MovieYearMin && number <= MovieYearMax)
                {
                    if (!_fileNameParser.Elements.ContainsKey(ElementCategory.MovieYear))
                    {
                        IdentifiedToken(ElementCategory.MovieYear, token);
                        continue;
                    }
                }

                // Video resolution
                if (number == 480 || number == 720 || number == 1080)
                {
                    if (!_fileNameParser.Elements.ContainsKey(ElementCategory.VideoResolution))
                    {
                        IdentifiedToken(ElementCategory.VideoResolution, token);
                    }
                }
            }
        }

        private void SearchForTitle()
        {
            var unidentifiedToken = _fileNameParser.Tokens.First(x => x.Category == TokenCategory.Unknown);
            
            var tokens = new List<Token>
            {
                unidentifiedToken
            };

            var tokenNode = _fileNameParser.Tokens.Find(unidentifiedToken);
            while (true)
            {
                if (tokenNode.Next == null)
                    break;

                tokenNode = tokenNode.Next;
                var token = tokenNode.Value;

                if (token.Category != TokenCategory.Delimiter && token.Category != TokenCategory.Unknown)
                    break;

                if (token.Category == TokenCategory.Unknown)
                    tokens.Add(token);
            }

            IdentifiedToken(ElementCategory.MovieTitle, tokens);
        }

        /// <summary>
        /// Gets the remaining unidentified <see cref="Token"/>s.
        /// </summary>
        /// <returns>Returns the remaining unidentified <see cref="Token"/>s.</returns>
        private IEnumerable<Token> GetRemainingTokens()
        {
            return _fileNameParser.Tokens.Where(x => x.Category == TokenCategory.Unknown);
        }
    }
}
