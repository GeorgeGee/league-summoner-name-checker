using SummonerNameChecker.Enums;
using SummonerNameChecker.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SummonerNameChecker
{
    public class ApiHelper
    {
        private HttpClient _httpClient;
        private string _apiKey;

        public ApiHelper(string apiKey, Server server) :
            this(apiKey, server.ToServerCode()) { }

        public ApiHelper(string apiKey, string serverCode)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Invalid Riot Games API Key", nameof(apiKey));

            if (string.IsNullOrEmpty(serverCode))
                throw new ArgumentException("Invalid server code", nameof(serverCode));

            _apiKey = apiKey;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"https://{serverCode}.api.riotgames.com/lol/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Summoner> GetSummoner(string summonerName)
        {
            if (summonerName.Length > 16)
                return new Summoner(summonerName, SummonerNameAvailability.TooLong);

            SummonerDto summonerDto = null;
            try
            {
                summonerDto = await GetSummonerDTOAsync(summonerName);
            }
            catch (RequestException re)
            {
                // could not find summoner
                if (re.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    return new Summoner(summonerName, SummonerNameAvailability.AvailableNotFound);
                else
                    return new Summoner(summonerName, SummonerNameAvailability.Unknown); // some other status code
            }
            
            try
            {
                var matchListDto = await GetMatchListDTOAsync(summonerDto.AccountId);

                return new Summoner(
                    summonerDto.Name,
                    summonerDto.SummonerLevel,
                    summonerDto.Id,
                    summonerDto.AccountId,
                    DateTimeOffset.FromUnixTimeMilliseconds(matchListDto.Matches.First().Timestamp).UtcDateTime);
            }
            catch (RequestException e)
            {
                // could not find match history
                return new Summoner(summonerName, SummonerNameAvailability.Unknown);
            }
        }

        private async Task<SummonerDto> GetSummonerDTOAsync(string summonerName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"summoner/v4/summoners/by-name/{summonerName}?api_key={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<SummonerDto>();
            }
            else
                throw new RequestException(response);
        }

        private async Task<MatchListDto> GetMatchListDTOAsync(string accountId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"match/v4/matchlists/by-account/{accountId}?api_key={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<MatchListDto>();
            }
            else
                throw new RequestException(response);
        }
    }
}
