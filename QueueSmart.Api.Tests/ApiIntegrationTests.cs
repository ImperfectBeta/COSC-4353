using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// Remove this from test project once Program is exposed in API project:
// public partial class Program { }

namespace QueueSmart.Api.Tests;

// ...existing code...

public sealed class ApiFactory : WebApplicationFactory<Program>
{
}

public class OpenApiSmokeTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public OpenApiSmokeTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OpenApi_AllDocumentedEndpoints_DoNotReturn500()
    {
        using var doc = await GetOpenApiDocumentAsync();
        var paths = doc.RootElement.GetProperty("paths");

        foreach (var path in paths.EnumerateObject())
        {
            var route = ReplacePathParams(path.Name);

            foreach (var operation in path.Value.EnumerateObject())
            {
                var method = ToHttpMethod(operation.Name);
                if (method is null) continue;

                using var request = new HttpRequestMessage(method, route);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (method == HttpMethod.Post || method == HttpMethod.Put || method.Method == "PATCH")
                {
                    request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                }

                var response = await _client.SendAsync(request);

                Assert.NotEqual(
                    HttpStatusCode.InternalServerError,
                    response.StatusCode);
            }
        }
    }

    private async Task<JsonDocument> GetOpenApiDocumentAsync()
    {
        var candidates = new[]
        {
            "/openapi/v1.json",
            "/swagger/v1/swagger.json",
            "/openapi.json"
        };

        foreach (var endpoint in candidates)
        {
            var response = await _client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) continue;

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json);
        }

        throw new InvalidOperationException("OpenAPI document endpoint not found.");
    }

    private static HttpMethod? ToHttpMethod(string method) => method.ToLowerInvariant() switch
    {
        "get" => HttpMethod.Get,
        "post" => HttpMethod.Post,
        "put" => HttpMethod.Put,
        "delete" => HttpMethod.Delete,
        "patch" => HttpMethod.Patch,
        "head" => HttpMethod.Head,
        "options" => HttpMethod.Options,
        _ => null
    };

    private static string ReplacePathParams(string path)
    {
        // Replace common route params with valid sample values
        return path
            .Replace("{id}", Guid.NewGuid().ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{serviceId}", Guid.NewGuid().ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{historyId}", Guid.NewGuid().ToString(), StringComparison.OrdinalIgnoreCase)
            .Replace("{userId}", Guid.NewGuid().ToString(), StringComparison.OrdinalIgnoreCase);
    }
}