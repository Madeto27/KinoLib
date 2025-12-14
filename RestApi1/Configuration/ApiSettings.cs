namespace KinoLib.Api.Configuration
{
    public class ApiSettings
    {
        public const string ApiSettingsSection = "ApiSettings";

        public bool EnableCaching { get; set; } = true;
        public int CacheDurationMinutes { get; set; } = 5;
    }
}