using RedNimbus.LambdaService;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LambdaService.Helper
{
    public enum LambdaStatusCode
    {
        Ok = 0,
        LambdaUnacceptableReturnValue,
        InternalError,
        NotAuthorized
    }

    public class LambdaReturnValue
    {
        public string Data { get; private set; }
        public LambdaStatusCode StatusCode { get; private set; }
        public LambdaReturnValue(LambdaStatusCode statusCode, string data)
        {
            StatusCode = statusCode;
            Data = data;
        }
    }
}
