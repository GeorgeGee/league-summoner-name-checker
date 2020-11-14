using SummonerNameChecker.Enums;
using SummonerNameChecker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SummonerNameCheckerConsole
{
    public static class TableGenerator
    {
        public static string GenerateTable(IEnumerable<Summoner> summoners)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" _________________________________________________________________________________________");
            sb.AppendLine("|    Summoner Name    |  Last Played (UTC)  | Available From (UTC) |      Available       |");
            sb.AppendLine("|---------------------|---------------------|----------------------|----------------------|");

            foreach (var summoner in summoners)
            {
                sb.AppendLine(String.Format("| {0,-19} | {1, -19} | {2, -20} | {3, -20} |",
                    summoner.NameAvailability == SummonerNameAvailability.TooLong ? $"{summoner.Name.Substring(0, 16)}.." : summoner.Name,
                    summoner.LastPlayedUtc?.ToString() ?? String.Empty,
                    summoner.AvailableOnUtc?.ToString() ?? String.Empty,
                    GetNameAvailabilityText(summoner)));
            }

            sb.Append("|_____________________|_____________________|______________________|______________________|");

            return sb.ToString();
        }

        public static string GenerateMinimalTable(IEnumerable<Summoner> summoners)
        {
            var sb = new StringBuilder();
            sb.AppendLine(" ______________________________________");
            sb.AppendLine("|    Summoner Name    |   Available    |");
            sb.AppendLine("|---------------------|----------------|");

            foreach (var summoner in summoners)
            {
                sb.AppendLine(String.Format("| {0,-19} | {1, -14} |",
                    summoner.NameAvailability == SummonerNameAvailability.TooLong ? summoner.Name.Substring(0, 16) + ".." : summoner.Name,
                    GetNameAvailabilityText(summoner)));
            }

            sb.Append("|_____________________|________________|");

            return sb.ToString();
        }

        private static string GetNameAvailabilityText(Summoner summoner)
        {
            if (summoner == null)
            {
                return "Unknown";
            }

            switch (summoner.NameAvailability)
            {
                case SummonerNameAvailability.Unavailable:
                    return "No";
                case SummonerNameAvailability.AvailableNotFound:
                    return "Yes";
                case SummonerNameAvailability.AvailableExpired:
                    return "Yes (expired)";
                case SummonerNameAvailability.TooLong:
                    return "Name too long";
                case SummonerNameAvailability.UnknownNeverPlayed:
                    return "Maybe (never played)";
                case SummonerNameAvailability.Unknown:
                default:
                    return "Unknown";
            }
        }
    }
}
