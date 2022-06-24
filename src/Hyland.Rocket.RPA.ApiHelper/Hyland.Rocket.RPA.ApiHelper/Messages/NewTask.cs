namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    /// <summary>
    /// Data to create a new task
    /// </summary>
    public class NewTask
    {
        public int processId { get; set; }
        public string inputData { get; set; }
        public RpaTaskType type { get; set; }
        public string diversity { get; set; }
        public bool redoable { get; set; }
        public bool checkDiversity { get; set; }
    }
}
