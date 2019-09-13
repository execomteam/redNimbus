using RedNimbus.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LambdaService.Database
{
    public interface ILambdaManagment
    {
        Lambda GetLambdaById(string guid);
        Lambda GetLambdaByImageId(string imageId);
        void AddLambda(Lambda l);
        void DeleteLambda(Lambda l);
    }
}
