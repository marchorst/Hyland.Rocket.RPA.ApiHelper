namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
