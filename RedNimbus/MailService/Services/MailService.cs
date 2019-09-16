using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.MailService.Services
{
    class MailService : BaseService
    {
        private string _from;
        public MailService(string from) : base()
        {
            _from = from;
            Subscribe("SendMail", SendMail);
        }

        public void SendMail(NetMQMessage message)
        {
            Message<MailServiceMessage> msg = new Message<MailServiceMessage>(message);
            MailHelper.SendMail(_from, msg.Data.MailTo, msg.Data.Subject, msg.Data.Body);
        }


    }
}
