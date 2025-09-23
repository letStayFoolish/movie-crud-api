namespace MovieApi.Settings;

// will be used to read data from our previously created JWT Section of appsettings.json using the IOptions feature of ASP.NET Core.
public class JWT
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double DurationInMinutes { get; set; }
}
