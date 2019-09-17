using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either.Mappings
{ 
    public interface IEitherMapper
    {
        Either<IError, TDestination> Map<TDestination>(object source);
        Either<IError, TDestination> Map<TDestination>(object source, Action<TDestination> logAction);
    }
}
