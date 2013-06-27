using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionSniper.XMPP;

namespace Infrastructure.XMPP
{
    public class Chat
    {
        private readonly ChatManager _manager;

        readonly List<IChatListener> _messageListeners = new List<IChatListener>();

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

        public void AddMessageListener(IChatListener messageListener)
        {
            if(messageListener != null)
                _messageListeners.Add(messageListener);
        }

        internal void MessageReceived(Message message)
        {
            lock (_messageListeners)
            {
                _messageListeners.ForEach(l => l.ProcessMessage(this, message));                    
            }
        }

        public void SendMessage(Message message)
        {
            _manager.SendMessage(message, _participant);
        }

        public void SendMessage(string message)
        {
            _manager.SendMessage(new Message(message), _participant);
        }

        public void RemoveMessageTranslator(IChatListener listener)
        {
                Task.Run(() =>
                {
                    lock (_messageListeners)
                    {
                        _messageListeners.Remove(listener);                        
                    }
                });
        }
    }
}