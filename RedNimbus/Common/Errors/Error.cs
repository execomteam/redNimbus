using RedNimbus.Common.Enums;
using RedNimbus.Common.Interfaces;

namespace RedNimbus.Common.Errors
{
    public class Error : IError
    {
        private readonly string _message;

        public Error(string message)
        {
            _message = message;
        }
        public string Message { get; }
    }
}
