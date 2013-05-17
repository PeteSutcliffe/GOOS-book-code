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
            var @event = AuctionEvent.From(message.Body);
            var type = @event.Type;
            if (type == "CLOSE")
            {
                _listener.AuctionClosed();
            }
            else if (type == "PRICE")
            {
                _listener.CurrentPrice(@event.CurrentPrice, @event.Increment);
            }
        }

        private class AuctionEvent
        {
            private readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

            public string Type
            {
                get
                {
                    return Get("Event");
                }
            }

            public int CurrentPrice
            {
                get
                {
                    return GetInt("CurrentPrice");
                }
            }

            public int Increment
            {
                get
                {
                    return GetInt("Increment");
                }
            }

            private int GetInt(string fieldName)
            {
                return int.Parse(Get(fieldName));
            }

            private string Get(string fieldName)
            {
                return _fields[fieldName];
            }

            private void AddField(string field)
            {
                var pair = field.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                _fields.Add(pair[0].Trim(), pair[1].Trim());
            }

            public static AuctionEvent From(string messageBody)
            {
                var @event = new AuctionEvent();
                foreach (var field in FieldsIn(messageBody))
                {
                    @event.AddField(field);
                }

                return @event;
            }

            static IEnumerable<string> FieldsIn(string messageBody)
            {
                return messageBody.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }        
    }
}