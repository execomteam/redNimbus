using RedNimbus.DTO.Enums;
using RedNimbus.DTO.Interfaces;

namespace RedNimbus.DTO.Errors
{
    public class Error : IError
    {
        private readonly UserCreateErrorCode _code;

        public Error(UserCreateErrorCode code)
        {
            _code = code;
        }
        public UserCreateErrorCode Code { get; }
    }
}
