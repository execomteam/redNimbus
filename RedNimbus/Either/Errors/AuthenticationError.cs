using System;
using System.Collections.Generic;
using System.Text;
using RedNimbus.DTO.Enums;

namespace RedNimbus.Either.Errors
{
    public class AuthenticationError : IError
    {
        public string _message;
        public ErrorCode _code;

        public AuthenticationError(string message, ErrorCode code)
        {
            _message = message;
            _code = code;
        }
        public ErrorCode Code { get { return _code; } }

        public string Message { get { return _message; } }
    }
}
