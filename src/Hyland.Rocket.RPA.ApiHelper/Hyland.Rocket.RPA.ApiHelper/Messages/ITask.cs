namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using System.Collections.Generic;

    public interface ITask
    {
        int taskId { get; set; }
        string type { get; set; }
        string status { get; set; }
        string diversity { get; set; }
        string outputdata { get; set; }
        List<Run> runs { get; set; }
    }
}
