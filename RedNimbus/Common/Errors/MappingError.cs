using RedNimbus.Common.Enums;
using RedNimbus.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Common.Errors
{
    class MappingError : IError
    {
        private string _message; 

        public MappingError(string message)
        {
            _message = message;
        }
        public string Message { get; }
    }
}
