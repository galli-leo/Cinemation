using System;
using System.Collections.Generic;
using Cinemation.Parser.Elements;
using Cinemation.Parser.Extensions;
using Cinemation.Parser.Keywords;

namespace Cinemation.Parser.Tokens
{
    /// <summary>
    /// Tokenizes the <see cref="FileNameParser.FileName"/> and searches for the easy identifiable <see cref="ElementCategory"/>s.
    /// </summary>
    public class Tokenizer
    {
        private static readonly List<Tuple<char, char>> BracketPairs = new List<Tuple<char, char>>
        {
            Tuple.Create('(', ')'), // U+0028-U+0029 Parenthesis
            Tuple.Create('[', ']'), // U+005B-U+005D Square bracket
            Tuple.Create('{', '}'), // U+007B-U+007D Curly bracket
            Tuple.Create('\u300C', '\u300D'), // Corner bracket
            Tuple.Create('\u300E', '\u300F'), // White corner bracket
            Tuple.Create('\u3010', '\u3011'), // Black lenticular bracket
            Tuple.Create('\uFF08', '\uFF09') // Fullwidth parenthesis
        };

        private readonly FileNameParser _fileNameParser;

        public Tokenizer(FileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
        }

        public bool Tokenize()
        {
            TokenizeBrackets();

            return _fileNameParser.Tokens.Count != 0;
        }

        /// <summary>
        /// Called when a token is found.
        /// </summary>
        /// <param name="category">The category the <see cref="Token"/> belongs to.</param>
        /// <param name="enclosed">Determines whether the <see cref="Token"/> is enclosed within brackets.</param>
        /// <param name="range">The identified <see cref="TokenRange"/>.</param>
        private void FoundToken(TokenCategory category, bool enclosed, TokenRange range)
        {
            var token = new Token(category, range, enclosed);
            _fileNameParser.Tokens.AddLast(token);
        }

        /// <summary>
        /// First we check all the brackets in the <see cref="FileNameParser.FileName"/>
        /// All content not between brackets is sent to <see cref="TokenizeByPreidentified"/>.
        /// </summary>
        private void TokenizeBrackets()
        {
            var isOpen = false;
            var previousPosition = 0;
            var currentPosition = 0;
            var matchingBracket = '\0';

            while (currentPosition < _fileNameParser.FileName.Length)
            {
                currentPosition = isOpen ? _fileNameParser.FileName.IndexOf(matchingBracket, currentPosition) : FindOpeningBracket(currentPosition, out matchingBracket);

                if (currentPosition == -1)
                    currentPosition = _fileNameParser.FileName.Length;

                var range = new TokenRange(_fileNameParser.FileName, previousPosition, currentPosition);
                if (range.Length > 0)
                    TokenizeByPreidentified(isOpen, range);

                if (currentPosition != _fileNameParser.FileName.Length)
                {
                    FoundToken(TokenCategory.Bracket, true, new TokenRange(_fileNameParser.FileName, currentPosition, ++currentPosition));
                    isOpen = !isOpen;
                    previousPosition = currentPosition;
                }
            }
        }

        /// <summary>
        /// Searches for keywords which are easily identifiable and matched to an <see cref="ElementCategory"/>.
        /// </summary>
        /// <param name="enclosed">Determines whether the <see cref="TokenRange"/> is enclosed within brackets.</param>
        /// <param name="range">The unidentified <see cref="TokenRange"/>.</param>
        private void TokenizeByPreidentified(bool enclosed, TokenRange range)
        {
            var preidentifiedTokens = KeywordManager.Peek(_fileNameParser, range);
            var currentIndex = range.StartIndex;
            var subrange = new TokenRange(_fileNameParser.FileName, range.StartIndex);

            while (currentIndex < range.EndIndex)
            {
                foreach (var preidentifiedToken in preidentifiedTokens)
                {
                    if (preidentifiedToken.StartIndex == currentIndex)
                    {
                        if (subrange.Length > 0)
                            TokenizeByDelimiters(enclosed, subrange);

                        FoundToken(TokenCategory.Identifier, enclosed, preidentifiedToken);
                        subrange.StartIndex = preidentifiedToken.EndIndex;
                        currentIndex = subrange.StartIndex - 1;
                        break;
                    }
                }

                currentIndex++;
                subrange.EndIndex = currentIndex;
            }

            if (subrange.Length > 0)
                TokenizeByDelimiters(enclosed, subrange);
        }

        /// <summary>
        /// Searches for delimiters.
        /// </summary>
        /// <param name="enclosed">Determines whether the delimiter is enclosed within brackets.</param>
        /// <param name="range">The unidentified <see cref="TokenRange"/>.</param>
        private void TokenizeByDelimiters(bool enclosed, TokenRange range)
        {
            var delimiter = range.FindDelimiter();

            if (delimiter == '\0')
            {
                FoundToken(TokenCategory.Unknown, enclosed, range);
                return;
            }

            var delimiterIndex = 0;
            var prevDelimiterIndex = delimiterIndex;

            while (delimiterIndex != range.Length)
            {
                delimiterIndex = range.Content.IndexOf(delimiter, prevDelimiterIndex);

                if (delimiterIndex == -1)
                    delimiterIndex = range.Length;

                var startIndex = prevDelimiterIndex + range.StartIndex;
                var subrange = new TokenRange(_fileNameParser.FileName, startIndex, startIndex + delimiterIndex - prevDelimiterIndex);
                if (subrange.Length > 0)
                    FoundToken(TokenCategory.Unknown, enclosed, subrange);

                if (delimiterIndex != range.Length)
                {
                    FoundToken(TokenCategory.Delimiter, enclosed, new TokenRange(_fileNameParser.FileName, subrange.EndIndex, subrange.EndIndex + 1));

                    prevDelimiterIndex = ++delimiterIndex;
                }
            }
        }

        /// <summary>
        /// Finds the next opening bracket.
        /// </summary>
        /// <param name="currentPosition">Position to start searching from.</param>
        /// <param name="endingBracket">The closing bracket of the found opening bracket.</param>
        /// <returns>Returns the position of the next opening bracket or the length of <see cref="FileNameParser.FileName"/>.</returns>
        private int FindOpeningBracket(int currentPosition, out char endingBracket)
        {
            for (var i = currentPosition; i < _fileNameParser.FileName.Length; i++)
            {
                foreach (var bracketPair in BracketPairs)
                {
                    if (bracketPair.Item1 == _fileNameParser.FileName[i])
                    {
                        endingBracket = bracketPair.Item2;
                        return i;
                    }
                }
            }

            endingBracket = char.MinValue;

            return _fileNameParser.FileName.Length;
        }
    }
}
