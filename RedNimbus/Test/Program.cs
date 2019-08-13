using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.DTO;
using RedNimbus.API.Services;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Run();   
        }

        static async Task Run()
        {
            CommunicationService requestSender = new CommunicationService("http://localhost:61253/");
            CreateUserDto user = new CreateUserDto();
            user.FirstName = "Test";
            user.LastName = "Testic";
            user.Email = "test@test.com";
            user.Password = "Test123@";

            IActionResult result = await requestSender.Send<CreateUserDto, IActionResult>("api/user", user);
            Console.ReadKey();
        }
    }
}
