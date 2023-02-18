using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace UtilityBot.Services.ApiCallerServices;

//keep it now to probably use it later for other stuff requiring external apis
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
}