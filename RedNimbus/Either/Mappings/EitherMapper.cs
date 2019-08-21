using AutoMapper;
using Either;
using RedNimbus.DTO.Interfaces;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.Either
{
    public class EitherMapper
    {
        private IMapper _mapper;

        public EitherMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public  Either<IError, TDestination> Map<TDestination>(object source)
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
