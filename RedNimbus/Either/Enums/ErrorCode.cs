using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.Either.Enums
{
    public enum ErrorCode
    {
        NullReference = 0,
        FirstNameNullEmptyOrWhiteSpace, 
        LastNameNullEmptyOrWhiteSpace, 
        EmailWrongFormat,               
        EmailAlreadyUsed,               
        PasswordWrongFormat,            
        PasswordsDoNotMatch,                  
        IncorrectEmailOrPassword,       
        UserNotFound,                  
        UserNotRegistrated,          
        MappingError,                   
        InternalServerError,            
        AccountDeactivated,
        NumberOfBucketsExeeded,
        ListBucketContentError,
        CreateFolderError,
        DeleteFolderError,
        CreateBucketError,
        PutFileError,
        GetFileError,
        DeleteFileError,
        PhoneNumberWrongFormat
    }
}
