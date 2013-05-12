using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace AuctionSniper.XMPP
{
    public class ChatManager
    {
        readonly List<Action<Chat>> _chatListeners = new List<Action<Chat>>();

        //todo: use these 2 as a dictionary for multiple chats?
        private Chat _chat;

        private string _messageFrom;

        private readonly Connection _connection;

        private readonly MessageQueue _inputChannel;

        internal ChatManager(Connection connection, MessageQueue inputChannel)
        {
            _connection = connection;
            _inputChannel = inputChannel;
            _inputChannel.ReceiveCompleted += ReceiveMessage;
        }

        void ReceiveMessage(object sender, ReceiveCompletedEventArgs e)
        {
            var message = _inputChannel.EndReceive(e.AsyncResult);
            var bf = new BinaryFormatter();            
            var ms = new MemoryStream(message.Extension);
            var extension = (MessageInfo)bf.Deserialize(ms);

            switch (extension.MessageType)
            {
                case MessageInfo.MessageTypes.CreateChat:
                    CreateChatFrom(extension.MessageFrom);
                    break;
                case MessageInfo.MessageTypes.Message:
                    _chat.MessageReceived(new Message(message.Body.ToString()));
                    break;
            }
        }

        public void AddChatListener(Action<Chat> chatCreated)
        {
            _chatListeners.Add(chatCreated);
        }

        internal void SendMessage(Message message)
        {
            _connection.SendMessage(message, _messageFrom);
        }

        private void CreateChatFrom(string messageFrom)
        {
            _messageFrom = messageFrom;

            //todo: allow multiple chats
            if (_chat != null)
            {
                throw new InvalidOperationException("Chat already exists");
            }

            _chat = new Chat(this);
            _chatListeners.ForEach(c => c(_chat));
        }

        public Chat CreateChat(string recipient, Action<Chat, Message> listener)
        {
            _connection.SendCreateMessage(recipient);
            CreateChatFrom(recipient);
            _chat.AddMessageListener(listener);
            return _chat;
        }

        public void MessageReceived(Message message)
        {
            _chat.MessageReceived(message);
        }

        public void ReceiveChat(string recipient)
        {
            CreateChatFrom(recipient);
        }
    }
}