using RedNimbus.API.Models;
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

        public async Task<Response<TResponseData>> Send<TRequest, TResponseData>(string path, TRequest data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(path, data);
            Response<TResponseData> toReturn = new Response<TResponseData>();

            toReturn.Value = await response.Content.ReadAsAsync<TResponseData>();
            toReturn.StatusCode = response.StatusCode;

            return toReturn;
        }
    }
}
