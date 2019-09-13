using System;
using RedNimbus.Communication;
using RedNimbus.Messages;
namespace RedNimbus.LambdaService.Services.Interfaces
{
    public interface ILambdaHelper
    {
        Guid CreateLambda(Message<LambdaMessage> message);

        string ExecuteLambda(string id);
    }
}
