using System;
using System.Collections.Generic;

namespace AuctionSniper.XMPP
{
    public class Chat
    {
        private readonly ChatManager _manager;

        readonly List<Action<Chat, Message>> _messageListeners = new List<Action<Chat, Message>>();

        public Chat(ChatManager manager)
        {
            _manager = manager;
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
    }
}