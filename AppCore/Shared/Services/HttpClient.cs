using AppCore.Shared.Interfaces;
using Domain.DTOs.HttpClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace AppCore.Shared.Services
{
    public class HttpClient : IHttpClient
    {
        private IRestResponse _restResponse;
        private readonly Logger _logger;

        public HttpClient(IRestResponse restResponse,
            Logger logger)
        {
            _restResponse = restResponse;
            _logger = logger;
        }

        public async Task<HttpClientResp> Get(string url, string requestId, string header = null)
        {

            try
            {
                RestClient client = new RestClient(url);

                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");

                //Do Http Request
                _restResponse = await client.ExecuteAsync(request);

                _logger.LogInfo($"[HttpClient][Post] => {url};{Environment.NewLine}" +
                   $"Req Id => {requestId};{Environment.NewLine}" +
                   $"StatusCode => {_restResponse?.StatusCode};{Environment.NewLine}" +
                   $"Response => {_restResponse.Content}" +
                   $"ErrorIfAny => {_restResponse.ErrorException}");

            }
            catch (Exception ex)
            {
                _logger.LogError($"[HttpClient][Post][Err] => {ex.Message}| {JsonConvert.SerializeObject(ex.InnerException)} {Environment.NewLine}" +
                    $"ENDPOINT => {url};{Environment.NewLine}" +
                     $"StatusCode => {_restResponse?.StatusCode};{Environment.NewLine}" +
                    $"REQUEST ID => {requestId};{Environment.NewLine}" +
                    $"HTTPRequestError => {_restResponse.ErrorException}");
            }
            
            return new HttpClientResp()
            {
                StatusCode = Convert.ToInt32(_restResponse.StatusCode),
                Content = _restResponse.Content
            };
        }
    }
}
