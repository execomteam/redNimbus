using RedNimbus.Either.Enums;

namespace RedNimbus.Either.Errors
{
    public class FormatError : IError
    {
        private string _message;
        private ErrorCode _code;

        public FormatError(string message, ErrorCode code)
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
