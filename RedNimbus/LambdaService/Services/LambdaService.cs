using NetMQ;
using Newtonsoft.Json;
using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using System;
using RedNimbus.LambdaService.Services.Interfaces;
using RedNimbus.LambdaService.Helper;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace RedNimbus.LambdaService.Services
{
    class LambdaService : BaseService
    {
        ITokenManager _tokenManager;
        ILambdaHelper _lambdaHelper;

        public LambdaService(ITokenManager tokenManager, ILambdaHelper lambdaHelper) : base()
        {
            _tokenManager = tokenManager;
            _lambdaHelper = lambdaHelper;
            
            Subscribe("CreateLambda", HandleCreateLambda);
            Subscribe("GetLambda", HandleGetLambda);
            Subscribe("PostLambda", HandlePostLambda);
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

                SendMessage(responseMessage.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("An error occured while trying to create lambda.", ErrorCode.InternalServerError, requestMessage.Id);
            }
        }

        private void HandleGetLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> requestMessage = new Message<LambdaMessage>(obj);

            Message<LambdaResultMessage> responseMessage = new Message<LambdaResultMessage>("Response")
            {
                Id = requestMessage.Id,
                Data = new LambdaResultMessage()
                {
                    LambdaId = requestMessage.Data.Guid
                }
            };

            try
            {
                string result = _lambdaHelper.ExecuteGetLambda(requestMessage.Data.Guid);
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

        private void HandlePostLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> requestMessage = new Message<LambdaMessage>(obj);

            Message<LambdaResultMessage> responseMessage = new Message<LambdaResultMessage>("Response")
            {
                Id = requestMessage.Id,
                Data = new LambdaResultMessage()
                {
                    LambdaId = requestMessage.Data.Guid
                }
            };

            try
            {
                byte[] result = _lambdaHelper.ExecutePostLambda(requestMessage);
                responseMessage.Data.Result = JsonConvert.SerializeObject(new LambdaReturnValue(LambdaStatusCode.Ok, null));
                responseMessage.Bytes = new NetMQFrame(result);
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
