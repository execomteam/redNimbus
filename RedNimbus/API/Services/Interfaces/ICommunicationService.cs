using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    interface ICommunicationService
    {
        Task<TResponseData> Send<TRequestData, TResponseData>(string path, TRequestData data);
    }
}
