using System;
using System.Collections.Generic;

namespace AuctionSniper.XMPP
{
    public class Chat
    {
        private readonly ChatManager _manager;

        readonly List<Action<Chat, Message>> _messageListeners = new List<Action<Chat, Message>>();
        private readonly string _participant;

        public Chat(ChatManager manager, string participant)
        {
            _manager = manager;
            _participant = participant;
        }

        public string Participant
        {
            get { return _participant; }
        }

        public void AddMessageListener(Action<Chat, Message> messageListener)
        {
            _messageListeners.Add(messageListener);
        }

        internal void MessageReceived(Message message)
        {
            _messageListeners.ForEach(l => l(this, message));
        }

        public void SendMessage(Message message)
        {
            _manager.SendMessage(message);
        }

        public void SendMessage(string message)
        {
            _manager.SendMessage(new Message(message));
        }
    }
}