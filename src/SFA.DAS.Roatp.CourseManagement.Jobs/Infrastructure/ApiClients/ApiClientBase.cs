using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.Firewall;

namespace SFA.DAS.Roatp.CourseManagement.Jobs.Infrastructure.ApiClients
{
    /// <summary>
    /// Base class containing common functionality that all ApiClients should use.
    /// Includes functionality to write an error log entry for any unsuccessful API calls.
    /// Please read documentation on all methods.
    /// </summary>
    /// <typeparam name="AC">The inherited ApiClient.</typeparam>
    public abstract class ApiClientBase<AC>
    {
        protected const string _acceptHeaderName = "Accept";
        protected const string _contentType = "application/json";

        protected readonly HttpClient _httpClient;
        protected readonly ILogger<AC> _logger;

        protected ApiClientBase(HttpClient httpClient, ILogger<AC> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains(_acceptHeaderName))
            {
                _httpClient.DefaultRequestHeaders.Add(_acceptHeaderName, _contentType);
            }
        }

        /// <summary>
        /// HTTP GET to the specified URI
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (of type T).</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<T> Get<T>(string uri)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<T>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP POST to the specified URI
        /// </summary>
        /// <typeparam name="T">The type of the object to POST.</typeparam>
        /// <typeparam name="U">The type of the object to read.</typeparam>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (of type U).</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<U> Post<T, U>(string uri, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, Encoding.UTF8, _contentType)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<U>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Post} - {uri}");
                throw;
            }
        }

        private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
        {
            if (response?.RequestMessage != null && !response.IsSuccessStatusCode)
            {
                var callingMethod = new System.Diagnostics.StackFrame(1).GetMethod().Name;

                var httpMethod = response.RequestMessage.Method.ToString();
                var statusCode = (int)response.StatusCode;
                var reasonPhrase = response.ReasonPhrase;
                var requestUri = response.RequestMessage.RequestUri;

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiErrorMessage = responseContent;

                if (TryParseJson<ApiError>(responseContent, out var apiError) && !string.IsNullOrWhiteSpace(apiError?.Message))
                {
                    apiErrorMessage = apiError.Message;
                }

                _logger.LogError($"Method: {callingMethod} || HTTP {statusCode} {reasonPhrase} || {httpMethod}: {requestUri} || Message: {apiErrorMessage}");
            }
        }

        private static bool TryParseJson<T>(string json, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (JsonException)
            {
                // The JSON is a different type
                result = default;
                return false;
            }
        }
    }
}