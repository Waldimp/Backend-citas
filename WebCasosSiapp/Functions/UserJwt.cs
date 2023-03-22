using System.Text.Json;

namespace WebCasosSiapp.Functions;

public class UserJwt
{
    public static string? Get(string? token)
    {
        if (token == null) return null;
        var segmentos = token.Split('.');
        var payload = segmentos[1];
        var jsonByte = ParseToBase64(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonByte);
            
        return keyValuePairs?["name"].ToString();

    }
    
    private static byte[] ParseToBase64(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}