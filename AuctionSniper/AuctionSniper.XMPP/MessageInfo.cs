using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MSMQMessage = System.Messaging.Message;

namespace AuctionSniper.XMPP
{
    [Serializable]
    public class MessageInfo
    {
        public enum MessageTypes
        {
            CreateChat,
            Message
        }

        public MessageTypes MessageType { get; set; }
        public string MessageFrom { get; set; }
        public string MessageTo { get; set; }
    }
    
    public static class MessageExtensionExtensions
    {
        public static byte[] Serialize(this MessageInfo info)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, info);
            var extensionBytes = ms.ToArray();
            return extensionBytes;
        }

        public static MessageInfo DeserializeExtension(this MSMQMessage message)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream(message.Extension);

            var extension = (MessageInfo)bf.Deserialize(ms);
            return extension;
        }
    }
}