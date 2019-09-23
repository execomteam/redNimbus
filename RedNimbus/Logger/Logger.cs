using Google.Protobuf;
using NetMQ;
using RedNimbus.LogLibrary;
using RedNimbus.Messages;
using System;
using System.IO;

namespace RedNimbus.LoggerService
{
    public class Logger
    {
        private ILogReceiver _receiver;
        string _logFilePath;

        public Logger(string endpoint, string logFilePath)
        {
            _logFilePath = logFilePath;
            _receiver = new LogReceiver(endpoint, MessageHandler);
        }

        public Logger(ILogReceiver receiver)
        {
            _receiver = receiver;
        }

        public void Start()
        {
            _receiver.Start();
        }

        public void Stop()
        {
            _receiver.Stop();
        }

        private void MessageHandler(NetMQMessage message)
        {
            Console.WriteLine("Hello from messageHandler");
            using (var writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine(ComposeLogMessage(message));
            }
        }

        private string ComposeLogMessage(NetMQMessage message)
        {
            //get request id
            Guid requestId = new Guid(message.Pop().ToByteArray());

            //get log message
            LogMessage data = new LogMessage();
            data.MergeFrom(message.Pop().ToByteArray());

            //compose message format
            // date | time | id | type | origin | payload 
            return String.Format("{0} | {1} | reqId: {2} | type: {3} | origin: {4} | payload: {5}", data.Date, data.Time, requestId.ToString(), data.Type, data.Origin, data.Payload);
        }
    }
}
