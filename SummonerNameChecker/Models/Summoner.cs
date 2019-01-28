using System;

namespace SummonerNameChecker
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

            // calculate when name will become available
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
