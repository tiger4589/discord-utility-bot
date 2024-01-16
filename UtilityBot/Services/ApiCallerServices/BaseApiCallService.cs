using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using UtilityBot.Services.LoggingServices;

namespace UtilityBot.Services.ApiCallerServices;

public abstract class BaseApiCallService
{
    private readonly string _apiBaseUrl;

    public abstract string? ServiceUrl { get; set; }

    protected BaseApiCallService(IConfiguration configuration)
    {
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("Can't find Api Base URL!");
    }

    protected async Task CallApi(object data)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                string.Concat(_apiBaseUrl, ServiceUrl),
                JsonContent.Create(data));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected async Task<T?> RequestApi<T>(object data)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                string.Concat(_apiBaseUrl, ServiceUrl),
                JsonContent.Create(data));

            var result = await response.Content.ReadFromJsonAsync<T?>();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected async Task<T?> RequestApi<T>() where T : class
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                string.Concat(_apiBaseUrl, ServiceUrl), null);

            try
            {
                var result = await response.Content.ReadFromJsonAsync<T?>();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected async Task<T?> GetApiFromServiceUrl<T>(Dictionary<string, string>? headers = null) where T : class
    {
        try
        {
            using var httpClient = new HttpClient();

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            
            var response = await httpClient.GetAsync(ServiceUrl);

            try
            {
                var result = await response.Content.ReadFromJsonAsync<T?>();
                return result;
            }
            catch (Exception e)
            {
                await Logger.Log($"Error calling API - Message: {e.Message}");
                return null;
            }
        }
        catch (Exception e)
        {
            await Logger.Log($"Error calling API - Message: {e.Message}");
            return null;
        }
    }
}