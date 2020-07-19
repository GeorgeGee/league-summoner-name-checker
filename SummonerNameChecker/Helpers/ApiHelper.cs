using SummonerNameChecker.Enums;
using SummonerNameChecker.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SummonerNameChecker
{
    public class ApiHelper
    {
        private HttpClient _httpClient;
        private string _apiKey;
        private TimeSpan _timeout;

        public ApiHelper(string apiKey, Server server, TimeSpan timeout) :
            this(apiKey, server.ToServerCode(), timeout) { }

        public ApiHelper(string apiKey, string serverCode, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Invalid Riot Games API Key", nameof(apiKey));

            if (string.IsNullOrEmpty(serverCode))
                throw new ArgumentException("Invalid server code", nameof(serverCode));
            
            _apiKey = apiKey;
            _timeout = timeout;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"https://{serverCode}.api.riotgames.com/lol/");
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
                        summonerDto = await ApiRequestAsync<SummonerDto>($"summoner/v4/summoners/by-name/{summonerName}?api_key={_apiKey}", cts.Token);

                    }
                    catch (HttpRequestException e)
                    {
                        if (e.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            // 404 = summoner does not exist
                            return new Summoner(summonerName, SummonerNameAvailability.AvailableNotFound);
                        }
                        throw;
                    }

                    if (summonerDto == null)
                        return new Summoner(summonerName, SummonerNameAvailability.Unknown);

                    // Fetch Summoner's match history
                    MatchListDto matchListDto = null;
                    try
                    {
                        matchListDto = await ApiRequestAsync<MatchListDto>($"match/v4/matchlists/by-account/{summonerDto.AccountId}?api_key={_apiKey}", cts.Token);
                    }
                    catch (HttpRequestException e)
                    {
                        if (e.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                        {
                            // 404 = no matches played
                            return new Summoner(summonerName, SummonerNameAvailability.UnknownNeverPlayed);
                        }
                        throw;
                    }

                    if (matchListDto == null || !matchListDto.Matches.Any())
                        return new Summoner(summonerName, SummonerNameAvailability.UnknownNeverPlayed);

                    return new Summoner(
                        summonerDto.Name,
                        summonerDto.SummonerLevel,
                        summonerDto.Id,
                        summonerDto.AccountId,
                        DateTimeOffset.FromUnixTimeMilliseconds(matchListDto.Matches.First().Timestamp).UtcDateTime);
                }
            }
            catch (Exception e) when (e is HttpRequestException || e is OperationCanceledException)
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
            throw new HttpRequestException(response);
        }
    }
}
