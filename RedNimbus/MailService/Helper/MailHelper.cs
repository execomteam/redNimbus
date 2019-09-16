using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace RedNimbus.MailService.Helper
{
    static class MailHelper
    {
        public static bool SendMail(string from, string to, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(from, to, subject, body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
