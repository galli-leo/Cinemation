using System;
using System.IO;
using System.Threading.Tasks;
using Cinemation.Core.Indexers;
using NLog;
using NLog.Config;

namespace Cinemation.Core.Debug
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            // Configure logger
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "Config", "NLog.config"));

            Logger.Warn("Booting Cinemation.Core.Debug");

            Run(args).GetAwaiter().GetResult();
            
            Logger.Warn("Shutting down");

            Console.ReadLine();
        }

        private static async Task Run(string[] args)
        {
            await Indexer.SearchTorrents(new SearchData
            {
                MovieName = "Guardians of the Galaxy",
                MovieYear = 2014,
                ImdbCode = "tt2015381"
            });

            await Indexer.SearchTorrents(new SearchData
            {
                MovieName = "Spectre",
                MovieYear = 2015,
                ImdbCode = "tt2379713"
            });
        }
    }
}
