namespace SummonerNameChecker.Models.Dto
{
    /// <summary>
    /// Represents a Summoner returned from Riot Games API
    /// </summary>
    public class SummonerDto
    {
        public int ProfileIconId { get; set; }
        public string Name { get; set; }
        public string Puuid { get; set; }
        public long SummonerLevel { get; set; }
        public long RevisionDate { get; set; }
        public string Id { get; set; }
        public string AccountId { get; set; }
    }
}
