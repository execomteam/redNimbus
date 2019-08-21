using RedNimbus.DTO.Enums;

namespace RedNimbus.DTO.Interfaces
{
    public interface IError
    {
        UserCreateErrorCode Code { get; }
    }
}
