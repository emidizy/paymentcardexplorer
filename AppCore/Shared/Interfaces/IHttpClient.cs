using Domain.DTOs.HttpClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Shared.Interfaces
{
    public interface IHttpClient
    {
        Task<HttpClientResp> Get(string url, string requestId, string header = null);
    }
}
