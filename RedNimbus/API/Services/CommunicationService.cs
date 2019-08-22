using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Models;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.DTO.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
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

        public async Task<Either<IError, TSuccess>> Send<TRequest, TSuccess>(string path, TRequest data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsJsonAsync(path, data);
            var result = await response.Content.ReadAsAsync<TSuccess>();
            return result;
        }
    }
}
