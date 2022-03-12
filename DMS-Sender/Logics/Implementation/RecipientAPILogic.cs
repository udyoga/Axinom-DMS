using DMS_Sender.Logics.Interface;
using DMS_Sender.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DMS_Sender.Logics.Implementation
{
    public class RecipientAPILogic : IRecipientAPILogic
    {
        private readonly IOptionsMonitor<Configs> _config;
        public RecipientAPILogic(IOptionsMonitor<Configs> optionsMonitor)
        {
            _config = optionsMonitor;
        }

        public async Task<HttpResponseMessage> SendEncyptedFile(string jsonFile, AuthModel authModel)
        {
            using (var client = new HttpClient())
            {
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authModel.Username}:{authModel.Password}"));

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                var content = new StringContent(JsonConvert.SerializeObject(new PostObject() { jsoncipherText = jsonFile }), Encoding.UTF8, "application/json");
                return client.PostAsync(_config.CurrentValue.RecipientAPI + "Receiving/SendJsonFile", content).Result; 
                // result.StatusCode == HttpStatusCode.OK;                 
            }
        }
    }
}
