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

        [JsonProperty("runs")] public List<Run> Runs { get; set; }
    }
}
