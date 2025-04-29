using System.Collections;
using System.Text;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
namespace Helpers;
//{
internal static class Tools
{
    private const string ApiKey = "pk.be4ce2e76ebfce8455e92137f1e0ebd3";
      private static readonly HttpClient HttpClient = new HttpClient();

    public static bool TryGetCoordinates(string address, out (double Latitude, double Longitude) coordinates)
    {
        coordinates = (0, 0); // ברירת מחדל

        if (string.IsNullOrWhiteSpace(address))
            return false;

        string url = $"https://us1.locationiq.com/v1/search.php?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;

                JsonNode json = JsonNode.Parse(responseBody);

                var firstResult = json?.AsArray()?.FirstOrDefault();

                if (firstResult != null)
                {
                    coordinates = (
                        double.Parse(firstResult["lat"]?.ToString() ?? "0"),
                        double.Parse(firstResult["lon"]?.ToString() ?? "0")
                    );
                    return true;
                }
            }
        }

        return false;
    }



    //namespace Helpers;
    /// <summary>
    /// Extension method to convert an object's properties to a formatted string representation
    /// including nested collections
    /// </summary>
    /// <typeparam name="T">The type of the object</typeparam>
    /// <param name="t">The object instance</param>
    /// <returns>A string representation of the object's properties</returns>
    //internal static class Tools
    //{
    //    private const string ApiKey = "679a8da6c01a6853187846vomb04142";
    //    private static readonly HttpClient HttpClient = new HttpClient();

    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "null";// null guard

        Type type = t.GetType();// actual runtime type
        var props = type.GetProperties();// public instance properties

        var sb = new StringBuilder();
        sb.Append(type.Name).Append(" { ");// header

        for (int i = 0; i < props.Length; i++)
        {
            var prop = props[i];
            var val = prop.GetValue(t);
            sb.Append(prop.Name).Append(" = ");

            if (val == null)
            {
                sb.Append("null");// property is null
            }
            else if (val is IEnumerable col && val is not string)
            {
                sb.Append("[ ");// start collection
                bool first = true;
                foreach (var item in col)
                {
                    if (!first) sb.Append(", ");
                    sb.Append(item ?? "null");
                    first = false;
                }
                sb.Append(" ]");// end collection
            }
            else
            {
                sb.Append(val);// simple value
            }

            if (i < props.Length - 1)
                sb.Append(", ");// separator
        }

        sb.Append(" }");// footer
        return sb.ToString();
    }

    public static (double Latitude, double Longitude) GetLatitudLongitutes(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("הכתובת אינה תקינה.");
        }

        string url = $"https://us1.locationiq.com/v1/search.php?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        try
        {
            HttpResponseMessage response = HttpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string jsonResponse = response.Content.ReadAsStringAsync().Result;

            var results = JsonSerializer.Deserialize<LocationIqResponse[]>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // התעלמות מהבדלי אותיות
            });

            if (results == null || results.Length == 0)
            {
                throw new BO.BlDoesNotExistException("לא נמצאו קואורדינטות לכתובת זו.");
            }

            if (!double.TryParse(results[0].Lat, out double latitude) ||
                !double.TryParse(results[0].Lon, out double longitude))
            {
                throw new BO.InvalidException("שגיאה בהמרת קואורדינטות.");
            }

            return (latitude, longitude);
        }
        catch (HttpRequestException httpEx)
        {
            throw new BO.BlUnauthorizedActionException("שגיאה בבקשת הרשת: " + httpEx.Message);
        }
        catch (JsonException jsonEx)
        {
            throw new BO.InvalidException("שגיאה בפענוח JSON: " + jsonEx.Message);
        }
        catch (Exception ex) when (!(ex is BO.InvalidException || ex is BO.BlUnauthorizedActionException || ex is BO.BlUnexpectedSystemException))
        {
            throw new BO.InvalidException("שגיאה בעת שליפת קואורדינטות: " + ex.Message);
        }
    }

    private class LocationIqResponse
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; }

        [JsonPropertyName("lon")]
        public string Lon { get; set; }
    }
}
