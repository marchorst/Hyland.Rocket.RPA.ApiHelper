namespace Hyland.Rocket.RPA.ApiHelper.Routes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Messages;
    using Newtonsoft.Json;
    using RestSharp;
    using Task = Messages.Task;

    public class TasksRoute : IRoute
    {
        public string BearerToken { get; set; }
        public string DomainWithProtocol { get; set; }

        #region Get

        /// <summary>
        /// Get a task by TaskID
        /// </summary>
        /// <param name="task">Task object</param>
        /// <param name="ignoreSsl">Ignore SSL Validation</param>
        /// <returns>The task object</returns>
        public ITask Get(ITask task, bool ignoreSsl = true) => this.Get(task.TaskId, ignoreSsl);

        /// <summary>
        /// Get a task by TaskID
        /// </summary>
        /// <param name="taskid">The TaskID</param>
        /// <param name="ignoreSsl">Ignore SSL Validation</param>
        /// <returns>The task object</returns>
        public ITask Get(int taskid, bool ignoreSsl = true)
        {
            // Create RPA TASK
            var client = new RestClient(this.DomainWithProtocol + "/heart/api/tasks/" + taskid);
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", this.BearerToken);

            var response = client.Execute(request);
            Task result = null;
            try
            {
                var deserializer = JsonSerializer.Create();
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
        /// <param name="ignoreSsl">Ignore SSL Validation</param>
        /// <returns>Create task object</returns>
        public ITask Create(NewTask taskData, bool ignoreSsl = true) =>
            this.Create(taskData.ProcessId, taskData.InputData, taskData.Type, taskData.Diversity, taskData.Redoable,
                taskData.CheckDiversity, ignoreSsl);

        /// <summary>
        /// Create a new RPA Task
        /// </summary>
        /// <param name="processId">The RPA ProcessID</param>
        /// <param name="inputData">Input data as string (I.e. serialized DataTable)</param>
        /// <param name="type">RPA Task Type (PRO / DCO)</param>
        /// <param name="diversity">Uniquie Diversity, default is empty</param>
        /// <param name="redoable">Default is false</param>
        /// <param name="checkDiversity">Check diversity, a task will only be created if the diversity is unique. Default is false</param>
        /// <param name="ignoreSsl">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>The RPA TaskID</returns>
        public ITask Create(int processId, string inputData, RpaTaskType type = RpaTaskType.PRO, string diversity = "",
            bool redoable = false, bool checkDiversity = false, bool ignoreSsl = true)
        {
            var client = new RestClient(this.DomainWithProtocol + "/heart/api/tasks");
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", this.BearerToken);
            var newBody = new CreateRpaTaskBody()
            {
                CheckDiversity = checkDiversity,
                ProcessId = processId,
                Type = type == RpaTaskType.PRO ? "PRO" : "DCO",
                Amount = 1,
                Status = "pending",
                Priority = "Now",
                Source = "Hyland Rocket API World",
                InputData = inputData,
                Diversity = diversity,
                ExecutionDate = null,
                Redoable = redoable
            };

            request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
            var response = client.Execute(request);
            List<Task> results = null;
            try
            {
                var deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<List<Task>>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw new Exception("Could not create task: " + e.InnerException.Message);
            }

            return results.First();
        }

        #endregion

        #region Redo
        /// <summary>
        /// Redo a task
        /// </summary>
        /// <param name="task">Task object</param>
        /// <param name="ignoreSsl">Ignore SSL Validation</param>
        /// <returns></returns>
        public ITask Redo(ITask task, bool ignoreSsl = true) => Redo(task.TaskId, ignoreSsl);

        /// <summary>
        /// Redo a task
        /// </summary>
        /// <param name="taskId">The TaskID</param>
        /// <param name="ignoreSsl">Ignore SSL Validation</param>
        /// <returns></returns>
        public ITask Redo(int taskId, bool ignoreSsl = true)
        {
            var client = new RestClient(this.DomainWithProtocol + "/heart/api/tasks/" + taskId.ToString() + "/redo");
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", this.BearerToken);

            var response = client.Execute(request);
            ITask results = null;
            try
            {
                var deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<Task>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw new Exception("Could not redo task: " + e.InnerException.Message);
            }

            return results;
        }
        #endregion
    }
}
