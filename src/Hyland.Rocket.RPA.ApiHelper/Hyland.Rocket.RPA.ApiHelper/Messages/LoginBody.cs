namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    public class LoginBody
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("remember")]
        public bool Remember { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }
    }
}
