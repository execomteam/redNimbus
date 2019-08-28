using System;
using RedNimbus.NewUserService.Services;

namespace RedNimbus.NewUserService
{
    class Program
    {
        static void Main(string[] args)
        {
            Service userService = new Service();

            userService.Start();
        }
    }
}
