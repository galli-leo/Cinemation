using System;
using System.IO;
using System.Linq;
using Cinemation.Parser.Tokens;
using Newtonsoft.Json.Linq;

namespace Cinemation.Parser.Debug
{
    // TODO: Improve..
    public class Program
    {
        public static void Main(string[] args)
        {
            var file = File.ReadAllText("Data/movies.json");
            var movies = JArray.Parse(file);

            foreach (var movie in movies)
            {
                var fileName = movie["file_name"].ToString();
                var fileNameParser = new FileNameParser(fileName);
                fileNameParser.Parse();

                var identifiedCount = 1;
                var success = true;

                foreach (var element in fileNameParser.Elements)
                {
                    var jsonKey = CamelCaseToUnderscore(element.Key.ToString());

                    if (movie[jsonKey].Type != JTokenType.Integer &&
                        movie[jsonKey].Type != JTokenType.String)
                        throw new Exception($"Unknown element in test data '{jsonKey}'.");

                    if (!movie[jsonKey].ToString().Equals(element.Value))
                    {
                        if (success)
                        {
                            success = false;

                            WriteOutColor($"Oops, '{fileName}' was not parsed successfully.", ConsoleColor.Red);
                        }

                        WriteOutColor($"Mismatch | '{movie[jsonKey]}' => '{element.Value}'", ConsoleColor.Red);
                    }
                    else
                    {
                        identifiedCount++;
                    }
                }

                if (success && identifiedCount != movie.Count())
                {
                    success = false;
                    WriteOutColor($"Oops, '{fileName}' was not parsed successfully, we forgot to identify something.", ConsoleColor.Red);
                }

                if (success)
                {
                    WriteOutColor($"Parsed '{fileName}'.", ConsoleColor.DarkGreen);
                }
                else
                {
                    WriteOutColor("==================================== Tokens", ConsoleColor.DarkYellow);
                    foreach (var token in fileNameParser.Tokens)
                    {
                        if (token.Category == TokenCategory.Unknown)
                        {
                            WriteOutColor(token.ToString(), ConsoleColor.DarkRed);
                        }
                        else if (token.Category == TokenCategory.Identifier)
                        {
                            WriteOutColor(token.ToString(), ConsoleColor.DarkCyan);
                        }
                        else
                        {
                            WriteOutColor(token.ToString(), ConsoleColor.DarkGreen);
                        }
                    }

                    WriteOutColor("==================================== Elements", ConsoleColor.DarkYellow);
                    foreach (var pair in fileNameParser.Elements)
                    {
                        WriteOutColor($"{pair.Key,-17}| {pair.Value}", ConsoleColor.DarkCyan);
                    }

                    WriteOutColor("==================================== JSON Data", ConsoleColor.DarkYellow);
                    foreach (var jsonObj in movie)
                    {
                        WriteOutColor($"{jsonObj.Path.Split('.')[1],-17}| {jsonObj.First}", ConsoleColor.DarkCyan);
                    }

                    break;
                }
            }

            Console.WriteLine("Press a key to exit.");
            Console.ReadKey();
        }

        private static void WriteOutColor(string str, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ResetColor();
        }
        
        // http://stackoverflow.com/a/18781533/6182203
        private static string CamelCaseToUnderscore(string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
