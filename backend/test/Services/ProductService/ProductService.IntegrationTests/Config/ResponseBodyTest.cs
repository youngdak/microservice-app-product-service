using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProductService.IntegrationTests.Config;

public class ApiResponseTest : ApiResponseTest<string>
{
    public string? Data { get; set; }
    public string Status { get; set; }
    public string? Message { get; set; }
}

public class ApiResponseTest<T>
{
    public T? Data { get; set; }
    public string Status { get; set; }
    public string? Message { get; set; }
}

public class LowerCaseContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return base.ResolvePropertyName(propertyName.ToLower());
    }
}

public static class ResponseContentExtension
{
    public static async Task<ApiResponseTest<T>> ApiResponseAsync<T>(this HttpContent httpContent)
    {
        var jsonResult = await httpContent.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponseTest<T>>(jsonResult, new JsonSerializerSettings
        {
            ContractResolver = new LowerCaseContractResolver()
        });

        return result;
    }

    public static async Task<ApiResponseTest> ApiResponseAsync(this HttpContent httpContent)
    {
        var jsonResult = await httpContent.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponseTest>(jsonResult, new JsonSerializerSettings
        {
            ContractResolver = new LowerCaseContractResolver()
        });

        return result;
    }
}
