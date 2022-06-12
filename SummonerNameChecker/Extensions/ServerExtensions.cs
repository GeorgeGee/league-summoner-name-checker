using SummonerNameChecker.Enums;
using System;

namespace SummonerNameChecker.Extensions
{
    public static class ServerExtensions
    {

        public static string ToPlatformRoutingValue(this Server server)
        {
            switch (server)
            {
                case Server.EUWest:
                    return "euw1";
                case Server.EUNordicEast:
                    return "eun1";
                case Server.NorthAmerica:
                    return "na1";
                case Server.Brazil:
                    return "br1";
                case Server.LatinAmericaNorth:
                    return "la1";
                case Server.LatinAmericaSouth:
                    return "la2";
                case Server.Oceania:
                    return "oc1";
                case Server.Russia:
                    return "ru";
                case Server.RepublicOfKorea:
                    return "kr";
                case Server.Japan:
                    return "jp1";
                case Server.Turkey:
                    return "tr1";
                default:
                    throw new ArgumentException("Unknown server");
            }
        }

        public static string ToRegionalRoutingValue(this Server server)
        {
            switch (server)
            {
                case Server.NorthAmerica:
                case Server.Brazil:
                case Server.LatinAmericaNorth:
                case Server.LatinAmericaSouth:
                case Server.Oceania:
                    return "americas";
                case Server.RepublicOfKorea:
                case Server.Japan:
                    return "asia";
                case Server.EUNordicEast:
                case Server.EUWest:
                case Server.Turkey:
                case Server.Russia:
                    return "europe";
                default:
                    throw new ArgumentException("Unknown server");
            }
        }
    }
}
