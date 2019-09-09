using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public abstract class BaseService
    {
        protected IError GetError(NetMQMessage response)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>(response);

            Either.Enums.ErrorCode errorCode = (Either.Enums.ErrorCode)Enum.ToObject(typeof(Either.Enums.ErrorCode), errorMessage.Data.ErrorCode);

            switch (errorCode)
            {
                //User service:
                case Either.Enums.ErrorCode.FirstNameNullEmptyOrWhiteSpace:
                case Either.Enums.ErrorCode.LastNameNullEmptyOrWhiteSpace:
                case Either.Enums.ErrorCode.EmailWrongFormat:
                case Either.Enums.ErrorCode.PasswordWrongFormat:
                case Either.Enums.ErrorCode.EmailAlreadyUsed:
                case Either.Enums.ErrorCode.PasswordsDoNotMatch:
                case Either.Enums.ErrorCode.PhoneNumberWrongFormat:
                //Bucket service:
                case Either.Enums.ErrorCode.NumberOfBucketsExeeded:
                    return new FormatError(errorMessage.Data.MessageText, errorCode);
                //User service:
                case Either.Enums.ErrorCode.IncorrectEmailOrPassword:
                    return new AuthenticationError(errorMessage.Data.MessageText, errorCode);
                //User service:
                case Either.Enums.ErrorCode.UserNotFound:
                case Either.Enums.ErrorCode.UserNotRegistrated:
                //Bucket service:
                case Either.Enums.ErrorCode.ListBucketContentError:
                case Either.Enums.ErrorCode.CreateFolderError:
                case Either.Enums.ErrorCode.CreateBucketError:
                case Either.Enums.ErrorCode.DeleteFolderError:
                case Either.Enums.ErrorCode.PutFileError:
                case Either.Enums.ErrorCode.GetFileError:
                case Either.Enums.ErrorCode.DeleteFileError:
                    return new NotFoundError(errorMessage.Data.MessageText, errorCode);
                default:
                    return new InternalServisError(errorMessage.Data.MessageText, errorCode);
            }
        }
    }
}
