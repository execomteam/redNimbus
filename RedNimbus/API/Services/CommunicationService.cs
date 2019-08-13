using RedNimbus.API.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly string _address;

        public CommunicationService(string address) {
            _address = address;
        }      

        public async Task<TResponseData> Send<TRequestData, TResponseData>(string path, TRequestData data)
            where TResponseData : IResponseData, new()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(path, data);
            TResponseData result = await response.Content.ReadAsAsync<TResponseData>();

            if(result == null)
            {
                result = new TResponseData();
            }

            result.StatusCode = response.StatusCode;

            return result;
        }
    }
}
