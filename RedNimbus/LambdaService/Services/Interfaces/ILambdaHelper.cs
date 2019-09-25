using System;
using RedNimbus.Communication;
using RedNimbus.Messages;
namespace RedNimbus.LambdaService.Services.Interfaces
{
    public interface ILambdaHelper
    {
        Guid CreateLambda(Message<LambdaMessage> lambdaMessage);

        string ExecuteGetLambda(string lambdaId);

        byte[] ExecutePostLambda(Message<LambdaMessage> lambdaMessage);
    }
}
