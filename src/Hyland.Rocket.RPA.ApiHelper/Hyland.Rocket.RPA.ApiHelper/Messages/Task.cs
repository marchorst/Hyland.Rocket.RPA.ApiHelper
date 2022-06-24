namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using System.Collections.Generic;

    public class Task : ITask
    {
        public int taskId { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string diversity { get; set; }
        public string outputdata { get; set; }
        public List<Run> runs { get; set; }
    }
}
