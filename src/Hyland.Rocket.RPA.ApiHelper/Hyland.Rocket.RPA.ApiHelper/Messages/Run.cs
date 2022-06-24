namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    public class Run
    {
        [JsonProperty("outputData")]
        public string OutputData { get; set; }
    }
}
