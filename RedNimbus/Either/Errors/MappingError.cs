using RedNimbus.DTO.Enums;
using RedNimbus.DTO.Interfaces;

namespace RedNimbus.Either.Errors
{
    public class MappingError : IError
    {
        private readonly string _message;

        public MappingError(string message)
        {
            _message = message;
        }
        public string Message { get; }
    }
}
