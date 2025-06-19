using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class VerificationProxyService
{

    private readonly IHttpClientFactory _clientFactory;

    public VerificationProxyService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> GetVerificationInitiationAsync(string userId)
    {
        var client = _clientFactory.CreateClient("Verification");
        var response = await client.GetAsync($"api/Verification/initiate-verification?userId={Uri.EscapeDataString(userId)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}