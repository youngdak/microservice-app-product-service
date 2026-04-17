using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace ProductService.IntegrationTests.Config;

public abstract class BaseApiIntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    protected readonly ApiWebApplicationFactory factory;
    protected HttpClient Client { get; private set; }

    protected BaseApiIntegrationTest(ApiWebApplicationFactory factory)
    {
        this.factory = factory;
        Client = factory.CreateClient();
    }
    protected string Authenticate(string sub)
    {
        var claims = new List<Claim> {
                new(ClaimTypes.NameIdentifier, sub)
            };

        var token = MockJwtTokens.GenerateJwtToken(claims);
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme, token);
        return token;
    }

    protected void Logout()
    {
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme, null);
    }

    protected async Task SaveJsonFakeAsync<T>(T data, string controller)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync($"/api/{controller}", content);

        await response.Content.ApiResponseAsync();
    }

    protected async Task SaveJsonFakeAsync<T>(T[] data, string controller)
    {
        foreach (var item in data)
        {
            await SaveJsonFakeAsync(item, controller);
        }
    }

    protected async Task<ApiResponseTest<TKey>> SaveJsonFakeAsync<T, TKey>(T data, string controller)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync($"/api/{controller}", content);

        var result = await response.Content.ApiResponseAsync<TKey>();

        return result;
    }

    protected async Task<ApiResponseTest<IEnumerable<T>>> GetFakesAsync<T>(string controller, Dictionary<string, string>? parameters = null) where T : class
    {
        var queryString = parameters != null && parameters.Any() ? QueryHelpers.AddQueryString("", parameters!) : "";
        var response = await Client.GetAsync($"/api/{controller}{queryString}");

        var data = await response.Content.ApiResponseAsync<IEnumerable<T>>();

        return data;
    }

    protected async Task<HttpResponseMessage> GetFakesAsync(string controller, Dictionary<string, string>? parameters = null)
    {
        var queryString = parameters != null && parameters.Any() ? QueryHelpers.AddQueryString("", parameters!) : "";
        var response = await Client.GetAsync($"/api/{controller}{queryString}");
        return response;
    }
}
