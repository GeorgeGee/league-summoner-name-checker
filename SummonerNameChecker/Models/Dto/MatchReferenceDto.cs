namespace SummonerNameChecker.Models.Dto
{
    /// <summary>
    /// Represents a Match returned from Riot Games API
    /// </summary>
    public class MatchReferenceDto
    {
        public string Lane { get; set; }
        public long GameId { get; set; }
        public int Champion { get; set; }
        public string PlatformId { get; set; }
        public int Season { get; set; }
        public int Queue { get; set; }
        public string Role { get; set; }
        public long Timestamp { get; set; }
    }
}
