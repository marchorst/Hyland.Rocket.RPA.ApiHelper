namespace Hyland.Rocket.RPA.ApiHelper.Routes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Hyland.Rocket.RPA.ApiHelper.Messages;
    using Newtonsoft.Json;
    using RestSharp;
    using Task = Hyland.Rocket.RPA.ApiHelper.Messages.Task;

    public class TasksRoute : IRoute
    {
        public string BearerToken { get; set; }
        public string DomainWithProtocol { get; set; }

        #region Get

        /// <summary>
        /// Get a task by TaskID
        /// </summary>
        /// <param name="task">Task object</param>
        /// <param name="ignoreSSL">Ignore SSL Validation</param>
        /// <returns>The task object</returns>
        public ITask Get(ITask task, bool ignoreSSL = true)
        {
            return Get(task.taskId, ignoreSSL);
        }

        /// <summary>
        /// Get a task by TaskID
        /// </summary>
        /// <param name="taskid">The TaskID</param>
        /// <param name="ignoreSSL">Ignore SSL Validation</param>
        /// <returns>The task object</returns>
        public ITask Get(int taskid, bool ignoreSSL = true)
        {
            // Create RPA TASK
            var client = new RestClient(DomainWithProtocol + "/heart/api/tasks/" + taskid);
            if (ignoreSSL)
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", BearerToken);

            var response = client.Execute(request);
            Task result = null;
            try
            {
                JsonSerializer deserializer = JsonSerializer.Create();
                result = deserializer.Deserialize<Task>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw new Exception("Could not find task: " + e.InnerException.Message);
            }

            return result;
        }

        #endregion

        #region Create

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="taskData">Task Data</param>
        /// <param name="ignoreSSL">Ignore SSL Validation</param>
        /// <returns>Create task object</returns>
        public ITask Create(NewTask taskData, bool ignoreSSL = true)
        {
            return Create(taskData.processId, taskData.inputData, taskData.type, taskData.diversity, taskData.redoable,
                taskData.checkDiversity, ignoreSSL);
        }

        /// <summary>
        /// Create a new RPA Task
        /// </summary>
        /// <param name="processId">The RPA ProcessID</param>
        /// <param name="inputData">Input data as string (I.e. serialized DataTable)</param>
        /// <param name="type">RPA Task Type (PRO / DCO)</param>
        /// <param name="diversity">Uniquie Diversity, default is empty</param>
        /// <param name="redoable">Default is false</param>
        /// <param name="checkDiversity">Check diversity, a task will only be created if the diversity is unique. Default is false</param>
        /// <param name="ignoreSSL">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>The RPA TaskID</returns>
        public ITask Create(int processId, string inputData, RpaTaskType type = RpaTaskType.PRO, string diversity = "",
            bool redoable = false, bool checkDiversity = false, bool ignoreSSL = true)
        {
            var client = new RestClient(DomainWithProtocol + "/heart/api/tasks");
            if (ignoreSSL)
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", BearerToken);
            var newBody = new CreateRpaTaskBody()
            {
                checkDiversity = checkDiversity,
                processId = processId,
                type = (type == RpaTaskType.PRO ? "PRO" : "DCO"),
                amount = 1,
                status = "pending",
                priority = "Now",
                source = "Hyland Rocket API World",
                inputData = inputData,
                diversity = diversity,
                executionDate = null,
                redoable = redoable
            };

            request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
            var response = client.Execute(request);
            List<Task> results = null;
            try
            {
                JsonSerializer deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<List<Task>>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw new Exception("Could not create task: " + e.InnerException.Message);
            }

            return results.First();
        }

        #endregion
    }
}
