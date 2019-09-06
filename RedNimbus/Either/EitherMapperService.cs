using Microsoft.Extensions.DependencyInjection;
using RedNimbus.Either.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either
{
    /// <summary>
    /// za dependency inj.
    /// </summary>
    public static class EitherMapperService
    {
        public static IServiceCollection AddEitherServiceMapper(this IServiceCollection collection)
        {
            collection.AddTransient<IEitherMapper, EitherMapper>();
            return collection;
        }
    }
}
