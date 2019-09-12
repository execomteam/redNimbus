﻿using NetMQ;
using Newtonsoft.Json;
using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RedNimbus.LambdaService
{
    class LambdaService : BaseService
    {
        ITokenManager _tokenManager;

        public LambdaService(ITokenManager tokenManager) : base()
        {
            _tokenManager = tokenManager;
            
            //subscriptions
            Subscribe("CreateLambda", HandleCreateLambda);
            Subscribe("GetLambda", HandleGetLambda);
        }

        private void HandleCreateLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>(obj);

            Guid guid = Utility.CreateLambda(message);

            message.Topic = "Response";
            message.Bytes = new NetMQFrame("");
            message.Data.Guid = guid.ToString();

            SendMessage(message.ToNetMQMessage());
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

            string result = null;
            responseMessage.Data.Token = requestMessage.Data.Token;
            responseMessage.Data.LambdaId = requestMessage.Data.LambdaId;

            try
            {
                result = ExecuteLambda(requestMessage.Data.LambdaId);
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

        private string ExecuteLambda(string id)
        {
            if(id == "1")
            {
                throw new ArgumentException();
            }else if(id == "2")
            {
                throw new Exception();
            }
            else
            {
                return "uspeo si jarane";
            }
        }

        private string ExecuteLambdaOriginal(string id)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = $"run --rm --name {id}container {id}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string result = string.Empty;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                //possible exception if lambda does not return string!!!
                result = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                if (!process.HasExited)
                    process.Kill();

                process.Close();
            }

            return result;
        }
    }
}
