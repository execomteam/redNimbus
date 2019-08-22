using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either.Errors
{
    public class UnacceptableFormatErr : IError
    {
        private string _message;
        public UnacceptableFormatErr()
        {
        }
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

          
    }
}
