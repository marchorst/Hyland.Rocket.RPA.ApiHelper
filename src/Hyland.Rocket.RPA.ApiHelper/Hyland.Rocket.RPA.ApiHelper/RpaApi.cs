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
        private DictionaryRoute dictionaries;
        public string BearerToken { get; set; }
        public string IdentityUrlWithProtocol { get; private set; }
        public string HeartUrlWithProtocol { get; private set; }
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
                    this.tasks = new TasksRoute()
                    {
                        BearerToken = this.BearerToken,
                        DomainWithProtocol = this.HeartUrlWithProtocol
                    };
                }

                return this.tasks;
            }
        }
        /// <summary>
        /// Get the TasksRoute CRUD
        /// </summary>
        public DictionaryRoute Dictionaries
        {
            get
            {
                if (this.dictionaries == null && !string.IsNullOrEmpty(this.BearerToken))
                {
                    this.dictionaries = new DictionaryRoute()
                    {
                        BearerToken = this.BearerToken,
                        DomainWithProtocol = this.HeartUrlWithProtocol
                    };
                }

                return this.dictionaries;
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
        [Obsolete("Use Access Token!")]
        public RpaApi(string domainWithProtocol, string clientId, string username, string password,
            bool getAccessToken = true)
        {
            this.HeartUrlWithProtocol = domainWithProtocol.EndsWith("/") ? domainWithProtocol.Remove(domainWithProtocol.Length - 1, 1) : domainWithProtocol;
            this.HeartUrlWithProtocol += "/heart";

            this.IdentityUrlWithProtocol = domainWithProtocol.EndsWith("/") ? domainWithProtocol.Remove(domainWithProtocol.Length - 1, 1) : domainWithProtocol;
            this.IdentityUrlWithProtocol += "/identity";

            this.ClientId = clientId;
            this.Username = username;
            this.Password = password;

            if (getAccessToken)
            {
                this.GetAccessToken();
            }
        }

        /// <summary>
        /// Initilize the Class
        /// </summary>
        /// <param name="heartUrlWithProtocol">Example: https://test.local/heart - NO ENDING /</param>
        /// <param name="identityUrlWithProtocol">Example: https://test.local/identity - NO ENDING /</param>
        /// <param name="clientId">The ClientID</param>
        /// <param name="username">A valid username</param>
        /// <param name="password">A valid Password</param>
        /// <param name="getAccessToken">Run the AccessToken method directly</param>
        [Obsolete("Use Access Token!")]
        public RpaApi(string heartUrlWithProtocol, string identityUrlWithProtocol, string clientId, string username, string password,
            bool getAccessToken = true)
        {
            this.HeartUrlWithProtocol = heartUrlWithProtocol.EndsWith("/") ? heartUrlWithProtocol.Remove(heartUrlWithProtocol.Length - 1, 1) : heartUrlWithProtocol;
            this.IdentityUrlWithProtocol = identityUrlWithProtocol.EndsWith("/") ? identityUrlWithProtocol.Remove(identityUrlWithProtocol.Length - 1, 1) : identityUrlWithProtocol;
            this.ClientId = clientId;
            this.Username = username;
            this.Password = password;

            if (getAccessToken)
            {
                this.GetAccessToken();
            }
        }

        /// <summary>
        /// Initilize the Class
        /// </summary>
        /// <param name="domainWithProtocol">Example: https://test.local - NO ENDING /</param>
        /// <param name="accessToken">Bearer Access Token</param>
        public RpaApi(string heartUrlWithProtocol, string identityUrlWithProtocol, string accessToken)
        {
            this.HeartUrlWithProtocol = heartUrlWithProtocol.EndsWith("/") ? heartUrlWithProtocol.Remove(heartUrlWithProtocol.Length - 1, 1) : heartUrlWithProtocol;
            this.IdentityUrlWithProtocol = identityUrlWithProtocol.EndsWith("/") ? identityUrlWithProtocol.Remove(identityUrlWithProtocol.Length - 1, 1) : identityUrlWithProtocol;
            this.BearerToken = accessToken.Trim().StartsWith("Bearer") ? accessToken.Trim() : "Bearer " + accessToken.Trim();
        }

        /// <summary>
        /// Get the Bearer Access BearerToken
        /// </summary>
        /// <param name="ignoreSsl">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>Bearer Access BearerToken</returns>
        public string GetAccessToken(bool ignoreSsl = true)
        {
            if (string.IsNullOrEmpty(this.BearerToken))
            {
                try
                {
                    var cookieContainer = new CookieContainer();
                    var client = new RestClient(this.IdentityUrlWithProtocol + "/api/Authorization/login");
                    if (ignoreSsl)
                    {
                        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    client.CookieContainer = new CookieContainer();
                    var request = new RestRequest(Method.POST);

                    request.AddHeader("Content-Type", "application/json");
                    var newBody = new LoginBody()
                    {
                        Identifier = this.Username,
                        Password = this.Password,
                        Remember = true,
                        RedirectUrl = this.IdentityUrlWithProtocol + "/swagger/oauth2-redirect.html"
                    };

                    request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
                    var response = client.Execute(request);
                    var id = response.StatusCode.ToString();

                    client = new RestClient(
                        $"{this.IdentityUrlWithProtocol}/connect/authorize?response_type=code&state=&client_id=" +
                        this.ClientId +
                        $"&scope={HttpUtility.UrlEncode("heart:group heart:process heart:application heart:instance heart:instanceSettings heart:agent heart:error heart:task heart:credentials heart:dictionary heart:activity")}&redirect_uri={HttpUtility.UrlEncode(this.HeartUrlWithProtocol + "/heart/swagger/oauth2-redirect.html")}");
                    if (ignoreSsl)
                    {
                        client.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    request = new RestRequest(Method.GET);
                    foreach (RestResponseCookie cookie in response.Cookies)
                    {
                        request.AddCookie(cookie.Name, cookie.Value);
                    }

                    response = client.Execute(request);
                    var code = response.ResponseUri.Query.Split('&').ToList().First(x => x.Contains("code="));
                    code = code.Split('=')[1];
                    client = new RestClient(this.IdentityUrlWithProtocol + "/connect/token");
                    if (ignoreSsl)
                    {
                        client.RemoteCertificateValidationCallback =
                            (sender, certificate, chain, sslPolicyErrors) => true;
                    }

                    request = new RestRequest(Method.POST);
                    foreach (RestResponseCookie cookie in response.Cookies)
                    {
                        request.AddCookie(cookie.Name, cookie.Value);
                    }

                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    request.AddParameter("grant_type", "authorization_code");
                    request.AddParameter("code", code);
                    request.AddParameter("redirect_uri",
                        this.HeartUrlWithProtocol + "/heart/swagger/oauth2-redirect.html");
                    request.AddParameter("client_id", this.ClientId);
                    response = client.Execute(request);
                    var serializer = JsonSerializer.Create();
                    var results = serializer.Deserialize<Token>(new JsonTextReader(new StringReader(response.Content)));
                    this.BearerToken = "Bearer " + results.AccessToken;
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
        /// <param name="ignoreSsl">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>The RPA TaskID</returns>
        [Obsolete("Use the TaskRoute")]
        public int CreateTask(int processId, string inputData, RpaTaskType type = RpaTaskType.PRO,
            string diversity = "", bool redoable = false, bool checkDiversity = false, bool ignoreSsl = true)
        {
            // Create RPA TASK
            var client = new RestClient(this.HeartUrlWithProtocol + "/heart/api/tasks");
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

            return results.First().TaskId;
        }
    }
}
