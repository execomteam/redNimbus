using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LogLibrary
{
    public interface ILogSender : IDisposable
    {
        void Start();
        void Stop();
        void Send(Guid requestId, LogMessage log);
    }
}
