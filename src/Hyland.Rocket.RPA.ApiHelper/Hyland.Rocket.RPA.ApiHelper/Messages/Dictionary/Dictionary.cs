namespace Hyland.Rocket.RPA.ApiHelper.Messages.Dictionary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class Dictionary : IDictionary
    {

        [JsonProperty("dictionaryId")]
        public int DictionaryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dictionaryEntries")]
        public List<DictionaryEntry> DictionaryEntries { get; set; }
    }
}
