using System;
using System.Net;

namespace SummonerNameChecker.Exceptions
{
    public class ApiRequestException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public ApiRequestException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
