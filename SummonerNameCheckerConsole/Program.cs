using CommandLine;
using SummonerNameChecker;
using SummonerNameChecker.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// TODO:
// 1. Add unit tests
//
// 2. Handle HttpStatusCode 429 (rate limit exceeded).
// Could add timers to wait (or retry every ~10s). Could check 429 and see if it tells you how long to wait?
// if (re.HttpResponseMessage.StatusCode == (HttpStatusCode)429)
//     Console.WriteLine(Convert.ToInt32(re.HttpResponseMessage.Headers.First().Value.First()));

namespace SummonerNameCheckerConsole
{
    public class Options
    {
        [Option('a', "apikey", Required = true, HelpText = "Riot Games API key. Get one for free at https://developer.riotgames.com")]
        public string ApiKey { get; set; }

        [Option('i', "input", Required = true, HelpText = "Input .txt file path")]
        public string InputFilePath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output .csv file path")]
        public string OutputFilePath { get; set; }

        [Option('s', "server", Required = false, HelpText = "League of Legends game server code. Default is \"euw1\" (EU West)", Default = "euw1")]
        public string Server { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
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
            ApiHelper apiHelper = null;
            var summoners = new List<Summoner>();
            try
            {
                apiHelper = new ApiHelper(options.ApiKey, options.Server);

                foreach (var name in names)
                {
                    var summoner = apiHelper.GetSummoner(name)
                        .GetAwaiter().GetResult();
                    summoners.Add(summoner);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while communicating with Riot Games API.\n{e.Message}");
            }

            if (summoners.Any())
            {
                if (!string.IsNullOrEmpty(options.OutputFilePath))
                {
                    try
                    {
                        CsvHelper.ExportToCsv(summoners, options.OutputFilePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while exporting summoners to CSV.\n{e.Message}");
                    }

                    var output = TableGenerator.GenerateTable(summoners);
                    Console.WriteLine(output);
                }
                else
                {
                    Console.WriteLine("No summoners were retrieved from the Riot Games API");
                }
            }
        }
    }
}