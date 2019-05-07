using System;
using System.Net.Http;

namespace SummonerNameChecker
{
    public class HttpRequestException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        public HttpRequestException(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
