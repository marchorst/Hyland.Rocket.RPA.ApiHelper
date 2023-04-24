namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Task : ITask
    {
        [JsonProperty("taskId")]
        public int TaskId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("diversity")]
        public string Diversity { get; set; }

        [JsonProperty("outputdata")]
        public string Outputdata { get; set; }


        [JsonProperty("closedAt")]
        public string closedAt { get; set; }

        [JsonProperty("createdAt")]
        public string createdAt { get; set; }

        [JsonProperty("process.name")]
        public string process { get; set; }

        [JsonProperty("runs")] public List<Run> Runs { get; set; }

        [JsonProperty("priority")]
        public string priority { get; set; }
    }
}
