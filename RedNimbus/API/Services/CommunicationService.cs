﻿using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly string _address;

        public CommunicationService(string address)
        {
            _address = address;
        }  

        public async Task<Either<IError, TSuccess>> Send<TRequest, TSuccess>(string path, TRequest data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Either<IError, TSuccess> result = null;
            HttpResponseMessage response = await client.PostAsJsonAsync(path, data);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<TSuccess>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                result = await response.Content.ReadAsAsync<FormatError>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                result = await response.Content.ReadAsAsync<NotFoundError>();
            }
            else if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                result = await response.Content.ReadAsAsync<AuthenticationError>();
            }
            else 
            {
                result = await response.Content.ReadAsAsync<InternalServisError>();
            }
            
            return result;
        }
    }
}
