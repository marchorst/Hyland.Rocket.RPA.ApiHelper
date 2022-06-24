namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data to create a new task
    /// </summary>
    public class NewTask
    {
        [JsonProperty("processId")]
        public int ProcessId { get; set; }

        [JsonProperty("inputData")]
        public string InputData { get; set; }

        [JsonProperty("type")]
        public RpaTaskType Type { get; set; }

        [JsonProperty("diversity")]
        public string Diversity { get; set; }

        [JsonProperty("redoable")]
        public bool Redoable { get; set; }

        [JsonProperty("checkDiversity")]
        public bool CheckDiversity { get; set; }
    }
}
