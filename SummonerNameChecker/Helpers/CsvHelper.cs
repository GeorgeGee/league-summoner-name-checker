using CsvHelper;
using CsvHelper.Configuration;
using SummonerNameChecker.Models;
using System.Collections.Generic;
using System.IO;

namespace SummonerNameChecker.Helpers
{
    public static class CsvHelper
    {
        public static void ExportToCsv(List<Summoner> summoners, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer,
                new Configuration { IncludePrivateMembers = true }))
            {
                csv.Configuration.RegisterClassMap<SummonerMap>();
                csv.WriteRecords(summoners);
            }
        }
    }

    public sealed class SummonerMap : ClassMap<Summoner>
    {
        public SummonerMap()
        {
            Map(m => m.Name);
            Map(m => m.Level);
            Map(m => m.LastPlayedUtc);
            Map(m => m.AvailableOnUtc);
            Map(m => m.NameAvailability);
        }
    }
}
