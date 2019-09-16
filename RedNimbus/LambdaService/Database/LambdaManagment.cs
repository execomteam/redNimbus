using System;
using System.Collections.Generic;
using System.Text;
using RedNimbus.Domain;
using System.Linq;
using AutoMapper;

namespace RedNimbus.LambdaService.Database
{
    public class LambdaManagment : ILambdaManagment
    {
        private IMapper _mapper;

        public LambdaManagment(IMapper mapper)
        {
           this._mapper = mapper;
        }
        public Lambda GetLambdaById(string guid)
        {
            LambdaDB lambda = new LambdaDB();

            using (var context = new LambdaContext())
            {
                lambda = context.Lambdas.First(l => l.Equals(guid));
            }

            if(lambda == null)
            {
                return null;
            }
            
            return _mapper.Map<Lambda>(lambda);
        }

        public void AddLambda(Lambda l)
        {
            LambdaDB lambda = _mapper.Map<LambdaDB>(l);

            using (var context = new LambdaContext())
            {
                context.Database.EnsureCreated();
                context.Add(lambda);
                context.SaveChanges();
            }
        }

        public void DeleteLambda(Lambda l)
        {
            using(var context = new LambdaContext())
            {
                LambdaDB lambda = context.Lambdas.First();
                context.Lambdas.Remove(lambda);
                context.SaveChanges();
            }
        }
    }
}
