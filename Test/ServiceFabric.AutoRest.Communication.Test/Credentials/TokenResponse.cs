using System;

namespace ServiceFabric.AutoRest.Communication.Test.Credentials
{
    internal class TokenResponse
    {
        internal string Token { get; set; }
        internal DateTime Expiration { get; set; }
    }
}