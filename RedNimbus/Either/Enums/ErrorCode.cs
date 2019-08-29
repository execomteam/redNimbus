using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.Either.Enums
{
    public enum ErrorCode
    {
        NullReference = 0,
        EmailAlreadyUsed,
        EmailWrongFormat,
        PasswordWrongFormat,    //unproc.ent
        PasswordsDoNotMatch,    //unproc.ent       
        FirstNameNullEmptyOrWhiteSpace,
        LastNameNullEmptyOrWhiteSpace,
        InternalServerError,
        IncorrectEmailOrPassword,
        UserNotFound,
        UserNotRegistrated,
        MappingError
    }
}
