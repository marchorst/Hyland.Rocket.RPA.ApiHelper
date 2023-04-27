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

        public List<Dictionary> All(int page = 1, int pageSize = 50, string filter = "", bool ignoreSsl = true) {
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
    }
}
