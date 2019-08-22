using AutoMapper;
using RedNimbus.Either.Errors;
using System;

namespace RedNimbus.Either.Mappings
{
    public class EitherMapper : IEitherMapper
    {
        private IMapper _mapper;

        public EitherMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Either<IError, TDestination> Map<TDestination>(object source)
        {
            try
            {
                return new Right<IError, TDestination>(_mapper.Map<TDestination>(source));
            }
            catch(Exception e)
            {
                return new Left<IError, TDestination>(new MappingError(e.Message));
            }
        }
    }
}
