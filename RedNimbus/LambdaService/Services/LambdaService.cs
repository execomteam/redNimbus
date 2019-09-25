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
using static RedNimbus.Messages.LambdaMessage.Types;

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
            Subscribe("GetUserLambdas", HandleGetUserLambdas);
            Subscribe("ExecuteGetLambda", HandleExecuteGetLambda);
            Subscribe("ExecutePostLambda", HandleExecutePostLambda);
        }

        private void HandleCreateLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> requestMessage = new Message<LambdaMessage>(obj);
            Guid ownerGuid = _tokenManager.ValidateToken(requestMessage.Data.OwnerId);

            if(ownerGuid == null)
            {
                SendErrorMessage("Token is not valid.", ErrorCode.InternalServerError, requestMessage.Id);
                return;
            }

            Guid lambdaId = _lambdaHelper.CreateLambda(requestMessage);

            if(!lambdaId.Equals(Guid.Empty))
            {
                Lambda lambda = new Lambda()
                {
                    Name = requestMessage.Data.Name,
                    Trigger = requestMessage.Data.Trigger,
                    Runtime = requestMessage.Data.Runtime,
                    Guid = lambdaId.ToString(),
                    OwnerGuid = ownerGuid
                };
                
                _lambdaManagment.AddLambda(lambda);

                Message<LambdaMessage> responseMessage = new Message<LambdaMessage>("Response")
                {
                    Data = new LambdaMessage()
                    {
                        Name = lambda.Name,
                        Trigger = lambda.Trigger,
                        Runtime = lambda.Runtime,
                        Guid = lambda.Guid,
                        OwnerId = lambda.OwnerGuid.ToString()
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

        private void HandleGetUserLambdas(NetMQMessage obj)
        {
            Message<ListLambdasMessage> requestMessage = new Message<ListLambdasMessage>(obj);
            var result = _lambdaManagment.GetLambdasByUserGuid(_tokenManager.ValidateToken(requestMessage.Data.Token));

            if (result == null)
            {
                SendErrorMessage("An error occured while trying to list lambdas.", ErrorCode.InternalServerError, requestMessage.Id);
            }

            requestMessage.Topic = "Response";

            foreach (var l in result)
            {
                LambdaMessage responseLambda = new LambdaMessage()
                {
                    Name = l.Name,
                    Trigger =  l.Trigger,
                    Runtime = l.Runtime,
                    Guid = l.Guid
                };
                requestMessage.Data.Lambdas.Add(responseLambda);  
            }

            SendMessage(requestMessage.ToNetMQMessage());
        }

        private void HandleExecuteGetLambda(NetMQMessage obj)
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
                var lambda = _lambdaManagment.GetLambdaById(requestMessage.Data.Guid);

                if (lambda.Trigger != TriggerType.Get)
                    throw new ArgumentException();

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

        private void HandleExecutePostLambda(NetMQMessage obj)
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
                var lambda = _lambdaManagment.GetLambdaById(requestMessage.Data.Guid);

                if (lambda.Trigger != TriggerType.Post)
                    throw new ArgumentException();

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
