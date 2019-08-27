using System;

namespace RedNimbus.EventBus
{
    class Program
    {
        static void Main(string[] args)
        {
            EventBus eventBus = new EventBus();
            eventBus.Start();
        }
    }
}
