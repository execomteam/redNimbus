using RedNimbus.DTO.Enums;

namespace RedNimbus.Either.Errors
{
    public interface IError
    {
        ErrorCode Code { get; }
        string Message { get; }
    }
}
