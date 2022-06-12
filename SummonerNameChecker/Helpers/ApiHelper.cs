using SummonerNameChecker.Enums;
using SummonerNameChecker.Exceptions;
using SummonerNameChecker.Extensions;
using SummonerNameChecker.Models;
using SummonerNameChecker.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SummonerNameChecker.Helpers
{
    public class ApiHelper
    {
        private HttpClient _httpClient;
        private string _apiKey;
        private string _platformRoutingValue;
        private string _regionalRoutingValue;
        private TimeSpan _timeout;

        public ApiHelper(string apiKey, Server server, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Invalid Riot Games API Key", nameof(apiKey));
            
            _apiKey = apiKey;
            _platformRoutingValue = server.ToPlatformRoutingValue();
            _regionalRoutingValue = server.ToRegionalRoutingValue();
            _timeout = timeout;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Summoner> GetSummoner(string summonerName)
        {
            try
            {
                using (CancellationTokenSource cts = new CancellationTokenSource(_timeout))
                {
                    if (summonerName.Length > 16)
                        return new Summoner(summonerName, SummonerNameAvailability.TooLong);

                    // Fetch Summoner
                    SummonerDto summonerDto = null;
                    try
                    {
                        summonerDto = await ApiRequestAsync<SummonerDto>(SummonerDtoRequest(_platformRoutingValue, summonerName, _apiKey), cts.Token);
                    }
                    catch (ApiRequestException e)
                    {
                        if (e.StatusCode == HttpStatusCode.NotFound)
                        {
                            // 404 = summoner does not exist
                            return new Summoner(summonerName, SummonerNameAvailability.AvailableNotFound);
                        }
                        throw;
                    }

                    if (summonerDto == null)
                        return new Summoner(summonerName, SummonerNameAvailability.Unknown);

                    // Fetch IDs of Summoner's recent matches
                    List<string> matchIds = null;
                    try
                    {
                        matchIds = await ApiRequestAsync<List<string>>(MatchesRequest(_regionalRoutingValue, summonerDto.Puuid, _apiKey), cts.Token);
                    }
                    catch (ApiRequestException e)
                    {
                        if (e.StatusCode == HttpStatusCode.NotFound)
                        {
                            // 404 = no matches played
                            return new Summoner(summonerName, SummonerNameAvailability.UnknownNeverPlayed);
                        }
                        throw;
                    }

                    if (matchIds == null || !matchIds.Any())
                        return new Summoner(summonerName, SummonerNameAvailability.UnknownNeverPlayed);

                    // Fetch most recent game
                    MatchDto match = null;
                    try
                    {
                        match = await ApiRequestAsync<MatchDto>(MatchDtoRequest(_regionalRoutingValue, matchIds.First(), _apiKey), cts.Token);
                    }
                    catch (ApiRequestException e)
                    {
                        if (e.StatusCode == HttpStatusCode.NotFound)
                        {
                            // 404 = no matches played
                            return new Summoner(summonerName, SummonerNameAvailability.UnknownNeverPlayed);
                        }
                        throw;
                    }

                    return new Summoner(
                        summonerDto.Name,
                        summonerDto.SummonerLevel,
                        summonerDto.Id,
                        summonerDto.AccountId,
                        DateTimeOffset.FromUnixTimeMilliseconds(match.Info.GameStartTimestamp).UtcDateTime);
                }
            }
            catch (Exception e) when (e is ApiRequestException || e is OperationCanceledException)
            {
                return new Summoner(summonerName, SummonerNameAvailability.Unknown);
            }
        }

        private async Task<T> ApiRequestAsync<T>(string uri, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else if (response.StatusCode == (HttpStatusCode)429) // Rate limit exceeded
            {
                // default 1 second period if Retry-After header is not included - this is the case when the rate limit is enforced by the underlying service
                double retryPeriodSeconds = response.Headers?.RetryAfter?.Delta?.TotalSeconds ?? 1;

                // wait the advised time period before retrying
                await Task.Delay(TimeSpan.FromSeconds(retryPeriodSeconds), cancellationToken);
                return await ApiRequestAsync<T>(uri, cancellationToken);
            }
            // some other status code
            throw new ApiRequestException(response.StatusCode);
        }

        private static string SummonerDtoRequest(string route, string summonerName, string apiKey)
            => $"https://{route}.api.riotgames.com/lol/summoner/v4/summoners/by-name/{summonerName}?api_key={apiKey}";

        private static string MatchesRequest(string route, string puuid, string apiKey)
            => $"https://{route}.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?api_key={apiKey}";

        private static string MatchDtoRequest(string route, string matchId, string apiKey)
            => $"https://{route}.api.riotgames.com/lol/match/v5/matches/{matchId}?api_key={apiKey}";
    }
}
