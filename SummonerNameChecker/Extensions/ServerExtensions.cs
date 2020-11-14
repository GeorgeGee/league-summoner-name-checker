using SummonerNameChecker.Enums;
using System;

namespace SummonerNameChecker.Extensions
{
    public static class ServerExtensions
    {
        public static string ToServerCode(this Server server)
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
                case Server.Unknown:
                default:
                    throw new ArgumentException("Unable to find server code for specified server");
            }
        }

        public static Server ToServer(this string serverCode)
        {
            switch (serverCode)
            {
                case "euw1":
                    return Server.EUWest;
                case "eun1":
                    return Server.EUNordicEast;
                case "na1":
                    return Server.NorthAmerica;
                case "br1":
                    return Server.Brazil;
                case "la1":
                    return Server.LatinAmericaNorth;
                case "la2":
                    return Server.LatinAmericaSouth;
                case "oc1":
                    return Server.Oceania;
                case "ru":
                    return Server.Russia;
                case "kr":
                    return Server.RepublicOfKorea;
                case "jp1":
                    return Server.Japan;
                case "tr1":
                    return Server.Turkey;
                default:
                    return Server.Unknown;
            }
        }
    }
}
