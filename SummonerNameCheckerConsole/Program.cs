using CommandLine;
using SummonerNameChecker.Enums;
using SummonerNameChecker.Helpers;
using SummonerNameChecker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SummonerNameCheckerConsole
{
    public class Options
    {
        [Option('a', "apikey", Required = true, HelpText = "Riot Games API key. Get one for free at https://developer.riotgames.com")]
        public string ApiKey { get; set; }

        [Option('i', "input", Required = true, HelpText = "Input .txt file path for names to check, line-separated")]
        public string InputFilePath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output .csv file path for saving results to CSV")]
        public string OutputFilePath { get; set; }

        [Option('s', "server", Required = false, HelpText = "League of Legends game server code. Default is \"EUWest\" (EU West)", Default = Server.EUWest)]
        public Server Server { get; set; }

        [Option("sortby", Required = false, HelpText = "Sort the results in the table and/or CSV by a value. Options are \"none\" (don't sort), \"name\", \"lastplayed\", \"availablefrom\", and \"available\"", Default = "none")]
        public string SortBy { get; set; }

        [Option("sortorder", Required = false, HelpText = "Sorting order, to be used in conjunction with --sortby. Options are \"asc\" for ascending (default) and \"desc\" for descending", Default = "asc")]
        public string SortOrder { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var parser = new Parser(cfg =>
            {
                cfg.CaseInsensitiveEnumValues = true;
                cfg.HelpWriter = Console.Out;
            });
            ParserResult<Options> result = parser.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.NotParsed)
                return;
            Options options = ((Parsed<Options>)result).Value;

            // read file
            IEnumerable<string> names = null;
            try
            {
                names = File.ReadLines(options.InputFilePath)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .Select(s => s.Trim());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to read input file input file. '{options.InputFilePath}'\n{e.Message}");
                return;
            }

            if (!names.Any())
            {
                Console.WriteLine($"Input file is empty: '{options.InputFilePath}'");
                return;
            }

            Console.WriteLine($"Checking availability of {names.Count()} summoner names...\n");

            // retrieve summoners
            var summoners = new List<Summoner>();
            try
            {
                var apiHelper = new ApiHelper(options.ApiKey, options.Server, TimeSpan.FromMinutes(10)); // 10 min timeout per request - i.e. the maximum rate limit period even for a Production API Key

                foreach (var name in names)
                {
                    var summoner = await apiHelper.GetSummoner(name);
                    summoners.Add(summoner);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while communicating with Riot Games API.\n{e.Message}");
                return;
            }

            if (summoners.Any())
            {
                summoners = SortSummoners(summoners, options.SortBy, options.SortOrder);

                var output = TableGenerator.GenerateTable(summoners);
                Console.WriteLine(output);

                if (!string.IsNullOrEmpty(options.OutputFilePath))
                {
                    try
                    {
                        SummonerNameChecker.Helpers.CsvHelper.ExportToCsv(summoners, options.OutputFilePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while exporting summoners to CSV.\n{e.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No summoners were retrieved from the Riot Games API");
            }
        }

        private static List<Summoner> SortSummoners(List<Summoner> summoners, string sortBy, string sortOrder)
        {
            // default is ascending unless they specified descending
            bool sortbyDesc = sortOrder != null && sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

            switch (sortBy?.ToLower())
            {
                case "name":
                    return sortbyDesc
                        ? summoners.OrderByDescending(s => s.Name).ToList()
                        : summoners.OrderBy(s => s.Name).ToList();
                case "lastplayed":
                    return sortbyDesc
                        ? summoners.OrderByDescending(s => s.LastPlayedUtc).ToList()
                        : summoners.OrderBy(s => s.LastPlayedUtc).ToList();
                case "availablefrom":
                    return sortbyDesc
                        ? summoners.OrderByDescending(s => s.AvailableOnUtc).ToList()
                        : summoners.OrderBy(s => s.AvailableOnUtc).ToList();
                case "available":
                    return sortbyDesc
                        ? summoners.OrderByDescending(s => s.NameAvailability).ToList()
                        : summoners.OrderBy(s => s.NameAvailability).ToList();
                case "none":
                default:
                    return summoners;
            }
        }
    }
}