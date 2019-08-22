using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either.Errors
{
    public class InternalServisErr : IError
    {
        private string _message;
        public InternalServisErr(string message)
        {
            _message = message;
        }
        public string Message
        {
            get { return _message; }
        }
    }
}
