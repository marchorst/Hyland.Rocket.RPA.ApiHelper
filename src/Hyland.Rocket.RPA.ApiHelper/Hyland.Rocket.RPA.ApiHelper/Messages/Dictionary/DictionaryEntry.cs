namespace Hyland.Rocket.RPA.ApiHelper.Messages.Dictionary
{
    using Newtonsoft.Json;

    public class DictionaryEntry
    {

        [JsonProperty("dictionaryEntryId")]
        public int DictionaryEntryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
