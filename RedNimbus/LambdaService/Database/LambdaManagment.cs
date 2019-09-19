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

        public List<Lambda> GetLambdasByUserGuid(Guid userGuid)
        {
            List<LambdaDB> lambdaDbResult = null;
            using (var context = new LambdaContext())
            {
                var query = from l in context.Lambdas
                            where l.OwnerGuid == userGuid
                            select  l ;
                lambdaDbResult = query.ToList();
            }
            if (lambdaDbResult == null)
            {
                return null;
            }

            List<Lambda> result = new List<Lambda>();
            foreach(var l in lambdaDbResult)
            {
                result.Add(_mapper.Map<Lambda>(l));
            }
            return result;
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
