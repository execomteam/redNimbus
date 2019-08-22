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
        PasswordWrongFormat,
        PasswordsDoNotMatch,
        PasswordMustHaveUpperCase,
        PasswordMustHaveLowerCase,
        PasswordMustHaveSpecialCharacter,
        PasswordMustHaveDigit,
        PasswordTooShort,
        FirstNameNullEmptyOrWhiteSpace,
        LastNameNullEmptyOrWhiteSpace
    }
}
