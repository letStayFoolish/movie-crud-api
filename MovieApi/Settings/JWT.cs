namespace MovieApi.Settings;

// will be used to read data from our previously created JWT Section of appsettings.json using the IOptions feature of ASP.NET Core.
public class JWT
{
    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public double DurationInMinutes { get; init; }
    public int  DurationInDays { get; set; }
}
