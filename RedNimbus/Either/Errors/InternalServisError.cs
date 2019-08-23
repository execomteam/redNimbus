using System;
using System.Collections.Generic;
using System.Text;
using RedNimbus.DTO.Enums;

namespace RedNimbus.Either.Errors
{
    public class InternalServisError : IError
    {
        private string _message;
        private ErrorCode _code;

        public InternalServisError(string message, ErrorCode code)
        {
            _message = message;
            _code = code;
        }
        public string Message
        {
            get { return _message; }
        }

        public ErrorCode Code
        {
            get { return _code; }
        }
    }
}
