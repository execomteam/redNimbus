using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetMQ;
using Google.Protobuf;

namespace RedNimbus.Communication
{
    public class Message<T> where T : IMessage, new()
    {
        public string Topic { get; set; }

        public Guid Id { get; set; }

        public T Data { get; set; } = new T();

        public Message(NetMQMessage message)
        {
            Topic = message.Pop().ConvertToString();            // TODO: Define topic string format
            Id = new Guid(message.Pop().ToByteArray());

            NetMQFrame data = message.Pop();
            Data.MergeFrom(data.ToByteArray());
        }

        public NetMQMessage ToNetMQMessage()
        {
            NetMQMessage message = new NetMQMessage();

            message.Append(Topic);

            if (Id == Guid.Empty)
                message.AppendEmptyFrame();
            else
                message.Append(new NetMQFrame(Id.ToByteArray()));

            MemoryStream stream = new MemoryStream();
            Data.WriteTo(stream);

            NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
            message.Append(dataFrame);

            return message;
        }
    }
}
