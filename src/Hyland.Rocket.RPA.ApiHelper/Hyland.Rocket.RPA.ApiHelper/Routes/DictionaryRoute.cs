namespace Hyland.Rocket.RPA.ApiHelper.Routes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Hyland.Rocket.RPA.ApiHelper.Messages.Dictionary;
    using Newtonsoft.Json;
    using RestSharp;

    public class DictionaryRoute : IRoute
    {
        public string BearerToken { get; set; }
        public string DomainWithProtocol { get; set; }

        public List<Dictionary> All(int page = 1, int pageSize = 50, string filter = "", bool ignoreSsl = true)
        {
            // Create RPA TASK
            var client = new RestClient(this.DomainWithProtocol + "/api/Dictionaries");
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", this.BearerToken);
            request.AddParameter("Filters", filter);
            request.AddParameter("Page", page);
            request.AddParameter("PageSize", pageSize);
            var response = client.Execute(request);
            var result = new List<Dictionary>();
            try
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var deserializer = JsonSerializer.Create();
                    result = deserializer.Deserialize<List<Dictionary>>(new JsonTextReader(new StringReader(response.Content)));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not find task: " + e.InnerException.Message);
            }

            return result;
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
        /// <param name="ignoreSsl">Do not validate any SSL certificate (When using self signed certificate)</param>
        /// <returns>The RPA TaskID</returns>
        public IDictionary Create(string name, bool ignoreSsl = true)
        {
            var client = new RestClient(this.DomainWithProtocol + "/api/Dictionaries");
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", this.BearerToken);
            var newBody = new Dictionary()
            {
                Name = name
            };

            request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
            var response = client.Execute(request);
            Dictionary results = null;
            try
            {
                var deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<Dictionary>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw e;
            }

            return results;
        }

        public DictionaryEntry CreateDictionaryEntry(int dictionaryId, DictionaryEntry newBody, bool ignoreSsl = true)
        {
            var client = new RestClient(this.DomainWithProtocol + "/api/Dictionaries/" + dictionaryId + "/entries");
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", this.BearerToken);

            request.AddParameter("application/json", RpaHelper.ToJson(newBody), ParameterType.RequestBody);
            var response = client.Execute(request);
            DictionaryEntry results = null;
            try
            {
                var deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<DictionaryEntry>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw e;
            }

            return results;
        }

        public DictionaryEntry UpdateDictionaryEntry(int DictionaryId, int DictionaryEntryId, string name, object newValue, bool ignoreSsl = true)
        {
            var client = new RestClient(this.DomainWithProtocol + "/api/Dictionaries/" + DictionaryId + "/entries/" + DictionaryEntryId);
            if (ignoreSsl)
            {
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", this.BearerToken);

            request.AddParameter("application/json", RpaHelper.ToJson(new DictionaryEntry() { Name = name, Value = newValue }), ParameterType.RequestBody);
            var response = client.Execute(request);
            DictionaryEntry results = null;
            try
            {
                var deserializer = JsonSerializer.Create();
                results = deserializer.Deserialize<DictionaryEntry>(new JsonTextReader(new StringReader(response.Content)));
            }
            catch (Exception e)
            {
                throw e;
            }

            return results;
        }
    }
}
