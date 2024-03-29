namespace Hyland.Rocket.RPA.ApiHelper.Messages
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public interface ITask
    {
        int TaskId { get; set; }
        string Type { get; set; }
        string Status { get; set; }
        string Diversity { get; set; }
        string Outputdata { get; set; }
        string closedAt { get; set; }
        string createdAt { get; set; }
        string process { get; set; }
        string priority { get; set; }
        List<Run> Runs { get; set; }
    }
}
