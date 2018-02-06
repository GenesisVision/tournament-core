using Microsoft.AspNetCore.Http;
using System.Net;

namespace GenesisVision.Tournament.Core.Infrastructure
{
    public static class HttpContextFilters
    {
        public static bool IsLocalRequest(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress) ||
                   IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
        }
    }
}
