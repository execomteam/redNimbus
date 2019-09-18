using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RedNimbus.MailService.Services
{
    class MailService : BaseService
    {
        private string _from;
        private SmtpClient client;
        public MailService() : base()
        {
            _from = "rednimbusteam@gmail.com";
            client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_from, "petarstupar1987")
            };
            Subscribe("SendMail", SendMail);
        }

        public void SendMail(NetMQMessage message)
        {
            Message<MailServiceMessage> msg = new Message<MailServiceMessage>(message);
            client.Send(_from, msg.Data.MailTo, msg.Data.Subject, msg.Data.Body);
        }

    }
}
