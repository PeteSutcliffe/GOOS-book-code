using System.Collections.Generic;
using AuctionSniper.Domain;
using Infrastructure.XMPP;

namespace AuctionSniper.XMPP
{
    public class XMPPAuction : IAuction
    {
        private readonly Chat _chat;

        private const string ItemIdAsLogin = "auction-{0}";

        private readonly CompositeEventListeners _listeners = new CompositeEventListeners();

        public XMPPAuction(Connection connection, string itemId)
        {
            var translator = TranslatorFor(connection);

            _chat = connection.GetChatManager()
                                         .CreateChat(AuctionId(itemId), 
                                         translator);
            AddAuctionEventListener(new FailListener(_chat, translator));
        }

        private AuctionMessageTranslator TranslatorFor(Connection connection)
        {
            return new AuctionMessageTranslator(connection.User, _listeners);
        }

        public void Bid(int amount)
        {
            SendMessage(string.Format(ApplicationConstants.BidFormat, amount));
        }

        public void Join()
        {
            SendMessage(ApplicationConstants.JoinCommandFormat);
        }

        private void SendMessage(string message)
        {
            _chat.SendMessage(message);
        }

        public void AddAuctionEventListener(IAuctionEventListener eventListener)
        {
           _listeners.AddListener(eventListener);
        }

        private static string AuctionId(string itemId)
        {
            return string.Format(ItemIdAsLogin, itemId);
        }
    }

    public class CompositeEventListeners : IAuctionEventListener
    {
        readonly List<IAuctionEventListener> _listeners = new List<IAuctionEventListener>();

        public void AddListener(IAuctionEventListener listener)
        {
            _listeners.Add(listener);
        }

        public void AuctionClosed()
        {
            _listeners.ForEach(l => l.AuctionClosed());
        }

        public void CurrentPrice(int price, int increment, PriceSource bidder)
        {
            _listeners.ForEach(l => l.CurrentPrice(price, increment, bidder));
        }

        public void AuctionFailed()
        {
            _listeners.ForEach(l => l.AuctionFailed());
        }
    }

    public class FailListener : IAuctionEventListener
    {
        private readonly Chat _chat;
        private readonly AuctionMessageTranslator _auctionMessageTranslator;

        public FailListener(Chat chat, AuctionMessageTranslator auctionMessageTranslator)
        {
            _chat = chat;
            _auctionMessageTranslator = auctionMessageTranslator;
        }

        public void AuctionClosed()
        {
            
        }

        public void CurrentPrice(int price, int increment, PriceSource bidder)
        {
            
        }

        public void AuctionFailed()
        {
            _chat.RemoveMessageTranslator(_auctionMessageTranslator);
        }
    }
}