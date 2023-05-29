using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class Location
{
    public static async Task<string> GetIpAddressAsync()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync("https://api.ipify.org");

            if (response.IsSuccessStatusCode)
            {
                var ipAddress = await response.Content.ReadAsStringAsync();
                return ipAddress;
            }

            return null;
        }
    }

    public static async Task<JObject> GetUserLocationAsync(string ipAddress)
    {
        var apiKey = "at_9RRw4aT1LNOqS6ssmsx1mVvwiYTqA";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"https://geo.ipify.org/api/v1?apiKey={apiKey}&ipAddress={ipAddress}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(content);
                return result.Value<JObject>("location");
            }

            return null;
        }
    }
}