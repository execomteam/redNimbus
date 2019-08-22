using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.DTO.Enums
{
    public enum UserCreateErrorCode
    {
        NullReference = 0,
        EmailAlreadyUsed,
        EmailWrongFormat,
        PasswordWrongFormat,    //unproc.ent
        PasswordsDoNotMatch,    //unproc.ent
        PasswordMustHaveUpperCase,  //unproc.ent
        PasswordMustHaveLowerCase,  //unproc.ent
        PasswordMustHaveSpecialCharacter,   //unproc.ent
        PasswordMustHaveDigit,              //unproc.ent
        PasswordTooShort,                   //unproc.ent
        FirstNameNullEmptyOrWhiteSpace,
        LastNameNullEmptyOrWhiteSpace
    }
}
