using System.Collections.Generic;
using Cinemation.Parser.Elements;
using Cinemation.Parser.Tokens;

namespace Cinemation.Parser
{
    /// <summary>
    /// Extracts useful information from filenames of anime uploads.
    /// </summary>
    public class FileNameParser
    {

        public readonly string FileName;

        public readonly LinkedList<Token> Tokens;

        public readonly Dictionary<ElementCategory, string> Elements;

        public FileNameParser(string fileName)
        {
            FileName = fileName;
            Tokens = new LinkedList<Token>();
            Elements = new Dictionary<ElementCategory, string>();
        }

        public bool Parse()
        {
            // Clean.
            Tokens.Clear();
            Elements.Clear();

            // Parse.
            var tokenizer = new Tokenizer(this);
            if (!tokenizer.Tokenize())
                return false;

            var parser = new Parse.Parser(this);
            parser.Parse();

            return true;
        }
    }
}
