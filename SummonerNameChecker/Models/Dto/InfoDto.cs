using System.Collections.Generic;

namespace SummonerNameChecker.Models.Dto
{
    public class InfoDto
    {
        public long GameCreation { get; set; }
        public long GameDuration { get; set; }
        public long GameEndTimestamp { get; set; }
        public long GameId { get; set; }
        public string GameMode { get; set; }
        public string GameName { get; set; }
        public long GameStartTimestamp { get; set; }
        public string GameType { get; set; }
        public string GameVersion { get; set; }
        public int MapId { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public string PlatFormId { get; set; }
        public int QueueId { get; set; }
        public List<TeamDto> Teams { get; set; }
        public string TournamentCode { get; set; }
    }
}
