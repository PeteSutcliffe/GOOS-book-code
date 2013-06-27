using System;
using System.Collections.Generic;
using AuctionSniper.XMPP;

namespace Infrastructure.XMPP
{
    public interface IChatListener
    {
        void ProcessMessage(Chat chat, Message message);
    }

    public class ChatManager
    {
        readonly List<Action<Chat>> _chatListeners = new List<Action<Chat>>();

        private readonly Dictionary<string, Chat> _chats = new Dictionary<string, Chat>();

        private readonly Connection _connection;

        internal ChatManager(Connection connection)
        {
            _connection = connection;
        }

        public void AddChatListener(Action<Chat> chatCreated)
        {
            _chatListeners.Add(chatCreated);
        }

        public Chat CreateChat(string recipient, IChatListener listener)
        {
            _connection.SendCreateMessage(recipient);
            CreateChatFrom(recipient);
            _chats[recipient].AddMessageListener(listener);
            return _chats[recipient];
        }

        public void MessageReceived(Message message, string chatParticipant)
        {
            _chats[chatParticipant].MessageReceived(message);
        }

        public void ReceiveChat(string messageFrom)
        {
            CreateChatFrom(messageFrom);
        }

        internal void SendMessage(Message message, string participant)
        {
            _connection.SendMessage(message, participant);
        }

        private void CreateChatFrom(string messageFrom)
        {
            if (_chats.ContainsKey(messageFrom))
            {
                throw new InvalidOperationException("Chat already exists");
            }

            var chat = new Chat(this, messageFrom);
            _chatListeners.ForEach(c => c(chat));

            _chats.Add(messageFrom, chat);
        }        
    }
}