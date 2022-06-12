using System.Collections.Generic;

namespace SummonerNameChecker.Models.Dto
{
    public class PerksDto
    {
        public PerkStatsDto StatPerks { get; set; }
        public List<PerkStyleDto> Styles { get; set; }
    }
}
