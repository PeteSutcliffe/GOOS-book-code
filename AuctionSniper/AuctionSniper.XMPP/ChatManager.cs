using System;
using System.Collections.Generic;
using System.Messaging;

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

            _chat = new Chat(this, messageFrom);
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