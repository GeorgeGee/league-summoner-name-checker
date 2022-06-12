using System.Collections.Generic;

namespace SummonerNameChecker.Models.Dto
{
    public class TeamDto
    {
        public List<BanDto> Bans { get; set; }
        public ObjectivesDto Objectives { get; set; }
        public int TeamId { get; set; }
        public bool Win { get; set; }
    }
}

