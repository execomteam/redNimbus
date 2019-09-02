using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.DTO.Enums
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
        AccountDeactivated
    }
}
