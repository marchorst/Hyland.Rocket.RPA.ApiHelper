namespace Hyland.Rocket.RPA.ApiHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Net;
    using System.IO;
    using RestSharp;
    using Newtonsoft.Json;
    using Messages;
    using Routes;

    /// <summary>
    /// Access the RPA API and create new tasks
    /// </summary>
    public class RpaApi
    {
        private TasksRoute tasks;
        public string BearerToken { get; set; }
        public string DomainWithProtocol { get; private set; }
        public string ClientId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        /// <summary>
        /// Get the TasksRoute CRUD
        /// </summary>
        public TasksRoute Tasks
        {
            get
            {
                if (this.tasks == null && !string.IsNullOrEmpty(this.BearerToken))
                {
                    this.tasks = new TasksRoute() {BearerToken = this.BearerToken, DomainWithProtocol = this.DomainWithProtocol};
                }

                return this.tasks;
            }
        }

        /// <summary>
        /// Get the TasksRoute CRUD
        /// </summary>
        /// <returns></returns>
        public TasksRoute TasksRoute() => this.Tasks;

        /// <summary>
        /// Initilize the Class
        /// </summary>
        /// <param name="domainWithProtocol">Example: https://test.local - NO ENDING /</param>
        /// <param name="clientId">The ClientID</param>
        /// <param name="username">A valid username</param>
        /// <param name="password">A valid Password</param>
        /// <param name="getAccessToken">Run the AccessToken method directly</param>
        public RpaApi(string domainWithProtocol, string clientId, string username, string password,
            bool getAccessToken = true)
        {
            this.DomainWithProtocol = domainWithProtocol;
            this.ClientId = clientId;
            this.Username = username;
            this.Password = password;

            if (getAccessToken) this.GetAccessToken();
        }

        /// <summary>
        /// Get the Bearer Access BearerToken
        /// </summary>
        /// <param name="ignoreSSL">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>Bearer Access BearerToken</returns>
        public string GetAccessToken(bool ignoreSSL = true)
        {
            if (string.IsNullOrEmpty(this.BearerToken))
            {
                try
                {
                    var cookieContainer = new CookieContainer();
                    var client = new RestClient(this.DomainWithProtocol + "/identity/api/Authorization/login");
                    if (ignoreSSL)
                        client.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    client.CookieContainer = new CookieContainer();
                    var request = new RestRequest(Method.POST);

                    request.AddHeader("Content-Type", "application/json");
                    var newBody = new LoginBody()
                    {
                        identifier = this.Username,
                        password = this.Password,
                        remember = true,
                        redirectUrl = this.DomainWithProtocol + "/identity/swagger/oauth2-redirect.html"
                    };

                    request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
                    var response = client.Execute(request);
                    var id = response.StatusCode.ToString();

                    client = new RestClient(
                        $"{this.DomainWithProtocol}/identity/connect/authorize?response_type=code&state=&client_id=" + this.ClientId +
                        $"&scope={HttpUtility.UrlEncode("heart:group heart:process heart:application heart:instance heart:instanceSettings heart:agent heart:error heart:task heart:credentials heart:dictionary heart:activity")}&redirect_uri={HttpUtility.UrlEncode(this.DomainWithProtocol + "/heart/swagger/oauth2-redirect.html")}");
                    if (ignoreSSL)
                        client.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    request = new RestRequest(Method.GET);
                    foreach (RestResponseCookie cookie in response.Cookies)
                    {
                        request.AddCookie(cookie.Name, cookie.Value);
                    }

                    response = client.Execute(request);
                    var code = response.ResponseUri.Query.Split('&').ToList().First(x => x.Contains("code="));
                    code = code.Split('=')[1];
                    client = new RestClient(this.DomainWithProtocol + "/identity/connect/token");
                    if (ignoreSSL)
                        client.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    request = new RestRequest(Method.POST);
                    foreach (RestResponseCookie cookie in response.Cookies)
                    {
                        request.AddCookie(cookie.Name, cookie.Value);
                    }

                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("grant_type", "authorization_code");
                    request.AddParameter("code", code);
                    request.AddParameter("redirect_uri", this.DomainWithProtocol + "/heart/swagger/oauth2-redirect.html");
                    request.AddParameter("client_id", this.ClientId);
                    response = client.Execute(request);
                    JsonSerializer serializer = JsonSerializer.Create();
                    var results = serializer.Deserialize<Token>(new JsonTextReader(new StringReader(response.Content)));
                    this.BearerToken = "Bearer " + results.access_token;
                    return this.BearerToken;
                }
                catch (Exception e)
                {
                    throw new Exception("Could not get token: " + e.InnerException.Message);
                }
            }

            return this.BearerToken;
        }

        /// DEPRECATED
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
        [Obsolete("Use the TaskRoute")]
        public int CreateTask(int processId, string inputData, RpaTaskType type = RpaTaskType.PRO,
            string diversity = "", bool redoable = false, bool checkDiversity = false, bool ignoreSSL = true)
        {
            // Create RPA TASK
            var client = new RestClient(this.DomainWithProtocol + "/heart/api/tasks");
            if (ignoreSSL)
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", this.BearerToken);
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

            return results.First().taskId;
        }
    }
}
