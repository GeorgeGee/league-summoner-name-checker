using System.Collections.Generic;

namespace SummonerNameChecker.Models.Dto
{
    /// <summary>
    /// Represents a Match List returned from Riot Games API
    /// </summary>
    public class MatchListDto
    {
        public List<MatchReferenceDto> Matches { get; set; }
        public int TotalGames { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
