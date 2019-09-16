using NetMQ;
using Newtonsoft.Json;
using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using System;
using RedNimbus.LambdaService.Services.Interfaces;
using RedNimbus.LambdaService.Helper;
using RedNimbus.LambdaService.Database;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;
using RedNimbus.Domain;

namespace RedNimbus.LambdaService.Services
{
    class LambdaService : BaseService
    {
        ITokenManager _tokenManager;
        ILambdaHelper _lambdaHelper;
        ILambdaManagment _lambdaManagment;

        public LambdaService(ITokenManager tokenManager, ILambdaHelper lambdaHelper, ILambdaManagment lambdaManagment) : base()
        {
            _tokenManager = tokenManager;
            _lambdaHelper = lambdaHelper;
            _lambdaManagment = lambdaManagment;
            
            Subscribe("CreateLambda", HandleCreateLambda);
            Subscribe("GetLambda", HandleGetLambda);
        }

        private void HandleCreateLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> requestMessage = new Message<LambdaMessage>(obj);

            Guid lambdaId = _lambdaHelper.CreateLambda(requestMessage);

            if(!lambdaId.Equals(Guid.Empty))
            {
                Message<LambdaMessage> responseMessage = new Message<LambdaMessage>("Response")
                {
                    Data = new LambdaMessage()
                    {
                        Guid = lambdaId.ToString()
                    },
                    Id = requestMessage.Id
                };

                Lambda lambda = new Lambda()
                {
                    Name = responseMessage.Data.Name,
                    Trigger = responseMessage.Data.Trigger,
                    Runtime = responseMessage.Data.Runtime
                };

                _lambdaManagment.AddLambda(lambda);

                SendMessage(responseMessage.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("An error occured while trying to create lambda.", ErrorCode.InternalServerError, requestMessage.Id);
            }
        }

        private void HandleGetLambda(NetMQMessage obj)
        {
            Message<GetLambdaMessage> requestMessage = new Message<GetLambdaMessage>(obj);
            Message<GetLambdaMessage> responseMessage = new Message<GetLambdaMessage>("Response")
            {
                Id = requestMessage.Id
            };

            Guid userId = _tokenManager.ValidateToken(requestMessage.Data.Token);
            if (userId.Equals(Guid.Empty))
            {
                responseMessage.Data.Token = "";
                responseMessage.Data.LambdaId = requestMessage.Data.LambdaId;
                responseMessage.Data.Result = JsonConvert.SerializeObject(new LambdaReturnValue(LambdaStatusCode.NotAuthorized, null));
                SendMessage(responseMessage.ToNetMQMessage());
            }

            responseMessage.Data.Token = requestMessage.Data.Token;
            responseMessage.Data.LambdaId = requestMessage.Data.LambdaId;

            try
            {
                string result = _lambdaHelper.ExecuteLambda(requestMessage.Data.LambdaId);
                responseMessage.Data.Result = JsonConvert.SerializeObject(new LambdaReturnValue(LambdaStatusCode.Ok, result));
            }
            catch (ArgumentException)
            {
                responseMessage.Data.Result = JsonConvert.SerializeObject(new LambdaReturnValue(LambdaStatusCode.LambdaUnacceptableReturnValue, null));
            }
            catch (Exception)
            {
                responseMessage.Data.Result = JsonConvert.SerializeObject(new LambdaReturnValue(LambdaStatusCode.InternalError, null));           
            }

            SendMessage(responseMessage.ToNetMQMessage());
        }

        new private void SendErrorMessage(string messageText, ErrorCode errorCode, NetMQFrame idFrame)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error")
            {
                Data = new ErrorMessage
                {
                    MessageText = messageText,
                    ErrorCode = (int)errorCode
                },
                Id = idFrame,
                Bytes = new NetMQFrame("")
            };

            NetMQMessage msg = errorMessage.ToNetMQMessage();
            SendMessage(msg);
        }
    }
}
