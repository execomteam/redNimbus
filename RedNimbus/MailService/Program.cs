using System;

namespace RedNimbus.MailService
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.MailService mailService = new Services.MailService();
            mailService.Start();
        }
    }
}
