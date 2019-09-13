using AutoMapper;
using DTO;
using RedNimbus.Domain;
using RedNimbus.LambdaService.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace LambdaService.Mappings
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Lambda, CreateLambdaDto>();
            CreateMap<CreateLambdaDto, Lambda>();
            CreateMap<CreateLambdaDto, LambdaDB>();
            CreateMap<LambdaDB, CreateLambdaDto>();
            CreateMap<LambdaDB, Lambda>();
            CreateMap<Lambda, LambdaDB>();
        }
    }
}
