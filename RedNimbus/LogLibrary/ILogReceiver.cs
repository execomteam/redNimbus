using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LogLibrary
{
    public interface ILogReceiver : IDisposable
    {
        void Start();
        void Stop();
    }
}