namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// JSON body structure to create a new Task
    /// </summary>
    public class CreateRpaTaskBody
    {
        public int processId { get; set; }
        public string type { get; set; }
        public int amount { get; set; }
        public string status { get; set; }
        public string priority { get; set; }
        public string source { get; set; }
        public string inputData { get; set; }
        public string diversity { get; set; }
        public object executionDate { get; set; }
        public bool redoable { get; set; }
        public bool checkDiversity { get; set; }
    }
}
