namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// JSON body structure to create a new Task
    /// </summary>
    public class CreateRpaTaskBody
    {

        [JsonProperty("processId")]
        public int ProcessId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("inputData")]
        public string InputData { get; set; }

        [JsonProperty("diversity")]
        public string Diversity { get; set; }

        [JsonProperty("executionDate")]
        public object ExecutionDate { get; set; }

        [JsonProperty("redoable")]
        public bool Redoable { get; set; }
        [JsonProperty("checkDiversity")]
        public bool CheckDiversity { get; set; }
    }
}
