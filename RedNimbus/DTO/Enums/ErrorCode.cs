using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.DTO.Enums
{
    public enum ErrorCode
    {
        NullReference = 0,
        FirstNameNullEmptyOrWhiteSpace, //1
        LastNameNullEmptyOrWhiteSpace,  //2
        EmailWrongFormat,               //3
        EmailAlreadyUsed,               //4
        PasswordWrongFormat,            //5
        PasswordsDoNotMatch,            //6      
        IncorrectEmailOrPassword,       //7
        UserNotFound,                   //8
        UserNotRegistrated,             //9
        MappingError,                   //10
        InternalServerError             //11
    }
}
