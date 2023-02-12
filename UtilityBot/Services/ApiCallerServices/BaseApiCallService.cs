using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using UtilityBot.Contracts;

namespace UtilityBot.Services.ApiCallerServices;

public abstract class BaseApiCallService
{
    private readonly string _apiBaseUrl;

    public abstract string? ServiceUrl { get; set; }

    protected BaseApiCallService(IConfiguration configuration)
    {
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("Can't find Api Base URL!");
    }
    //todo: improve exceptions and probably add logging
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
}