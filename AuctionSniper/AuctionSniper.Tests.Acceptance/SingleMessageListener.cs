using System;
using AuctionSniper.XMPP;

namespace AuctionSniper.Tests.Acceptance
{
    public class SingleMessageListener
    {
        readonly ArrayBlockingQueue<Message> _messages = new ArrayBlockingQueue<Message>(1);

        public void ProcessMessage(Chat chat, Message message)
        {
            _messages.Add(message);
            Console.WriteLine("Message added to queue");
        }

        public void ReceivesAMessage()
        {
            Console.WriteLine("Checking Queue");
            Message message;
            if (!_messages.TryTake(out message, 5000))
            {
                throw new Exception("Message did not arrive");
            }
            Console.WriteLine("Found message");
        }
    }
}