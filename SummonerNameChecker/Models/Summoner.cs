using SummonerNameChecker.Enums;
using System;

namespace SummonerNameChecker.Models
{
    public class Summoner
    {
        public readonly string Name;
        public readonly long? Level;
        public readonly string SummonerId;
        public readonly string AccountId;
        public readonly DateTime? LastPlayedUtc;
        public readonly DateTime? AvailableOnUtc;
        public readonly SummonerNameAvailability NameAvailability;

        public Summoner(string summonerName, long level, string summonerId, string accountId, DateTime lastPlayedUtc)
        {
            Name = summonerName;
            Level = level;
            SummonerId = summonerId;
            AccountId = accountId;
            LastPlayedUtc = lastPlayedUtc;

            // Calculate when name will become available
            // *************************************************************************************************************************************
            // Each Summoner name will have 6 months of inactivity protection upon creation.
            // Each Summoner name will earn an additional month of inactivity protection for each Summoner level above 6, up to 30 months.
            // Inactivity is defined to be a period in which no games of any kind have been played on the account.
            // When inactivity protection expires, your name will be available to be claimed through the Summoner name Change service in the store.
            //
            // Source: https://support.riotgames.com/hc/en-us/articles/201751914-Inactive-Summoner-name-rules
            // *************************************************************************************************************************************
            var monthsForExpiry = Math.Max(6, Math.Min(Level.Value, 30)); // minimum 6 months protection, +1 month for each level, max 30
            var availableOnPrecise = LastPlayedUtc.Value.AddMonths((int)monthsForExpiry); // most recent game + months required for expiry
            AvailableOnUtc = availableOnPrecise.Date.Add(new TimeSpan(0, 0, 0)); // null out the time since Riot clean names up at midnight UTC

            NameAvailability = AvailableOnUtc <= DateTime.UtcNow ? SummonerNameAvailability.AvailableExpired : SummonerNameAvailability.Unavailable;
        }

        public Summoner(string summonerName, SummonerNameAvailability availabilityStatus)
        {
            Name = summonerName;
            NameAvailability = availabilityStatus;
        }
    }
}
