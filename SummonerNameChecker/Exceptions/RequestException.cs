using System;
using System.Net.Http;

namespace SummonerNameChecker
{
    public class RequestException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        public RequestException(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
