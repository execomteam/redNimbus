using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using LambdaService.Mappings;
using RedNimbus.LambdaService.Database;
using System;

namespace RedNimbus.LambdaService
{
    class Program
    {
        static void Main(string[] args)
        {
            //simpleioc
            LambdaService lambdaService = new LambdaService();
            lambdaService.Start();
        }
    }
}
