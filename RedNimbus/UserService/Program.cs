using System;

namespace RedNimbus.UserService
{
    class Program
    {
        static void Main(string[] args)
        {
            UserService userService = new UserService();
            userService.Start();
        }
    }
}
