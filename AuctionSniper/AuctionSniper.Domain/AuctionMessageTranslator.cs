using System;
using System.Collections.Generic;
using AuctionSniper.XMPP;

namespace AuctionSniper.Domain
{
    public class AuctionMessageTranslator
    {
        private readonly IAuctionEventListener _listener;

        public AuctionMessageTranslator(IAuctionEventListener listener)
        {
            _listener = listener;
        }

        public void ProcessMessage(Chat chat, Message message)
        {
            var @event = UnpackEventFrom(message);
            var type = @event["Event"];
            if (type == "CLOSE")
            {
                _listener.AuctionClosed();
            }
            else if (type == "PRICE")
            {
                _listener.CurrentPrice(int.Parse(@event["CurrentPrice"]), int.Parse(@event["Increment"]));
            }
        }

        private Dictionary<string, string> UnpackEventFrom(Message message)
        {
            var @event = new Dictionary<string, string>();

            foreach (var element in message.Body.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = element.Split(':');
                @event.Add(pair[0].Trim(), pair[1].Trim());
            }
            return @event;
        }
    }
}