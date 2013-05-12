using System;
using AuctionSniper.XMPP;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace AuctionSniper.Tests.Acceptance
{
    public class SingleMessageListener
    {
        readonly ArrayBlockingQueue<Message> _messages = new ArrayBlockingQueue<Message>(1);

        public void ProcessMessage(Chat chat, Message message)
        {
            _messages.Add(message);
        }

        public void ReceivesAMessage(IResolveConstraint messageMatcher)
        {
            Message message;
            if (!_messages.TryTake(out message, 5000))
            {
                throw new Exception("Message did not arrive");
            }
            //Assert.That(message.Body, messageMatcher);
        }
    }
}