using System;
using System.IO;
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

            Indexer.SearchTorrents("Spectre (2016)");

            Logger.Warn("Shutting down");
            Console.ReadLine();
        }
    }
}
