using System;
using System.IO;
using NetMQ;
using Google.Protobuf;

namespace RedNimbus.Communication
{
    /// <summary>
    /// Message used for communication between services. 
    /// </summary>
    /// <typeparam name="T">The generic type parameter must implement the Google.Protobuf.IMessage interface
    /// and have a public parameterless constructor.</typeparam>
    public class Message<T> where T : IMessage, new()
    {
        /// <summary>
        /// Represents the topic this message is related to.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Represents the NetMQFrame that contains the unique identifer of the request connection.
        /// </summary>
        public NetMQFrame Id { get; set; }

        /// <summary>
        /// Represents the payload of the message.
        /// </summary>
        public T Data { get; set; } = new T();

        /// <summary>
        /// Create a new Message instance.
        /// </summary>
        /// <param name="message">Instance of NetMQMessage class.</param>
        public Message(NetMQMessage message)
        {
            Topic = message.Pop().ConvertToString();

            Id = message.Pop();

            NetMQFrame data = message.Pop();
            Data.MergeFrom(data.ToByteArray());
        }

        /// <summary>
        /// Converts the object to a NetMQMessage.
        /// </summary>
        /// <returns>Returns a new NetMQMessage instance.</returns>
        public NetMQMessage ToNetMQMessage()
        {
            NetMQMessage message = new NetMQMessage();

            message.Append(Topic);

            message.Append(Id);

            MemoryStream stream = new MemoryStream();
            Data.WriteTo(stream);

            NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
            message.Append(dataFrame);

            return message;
        }
    }
}
