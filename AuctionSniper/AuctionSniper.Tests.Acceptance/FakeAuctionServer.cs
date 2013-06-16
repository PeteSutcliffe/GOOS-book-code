using AuctionSniper.Domain;
using AuctionSniper.XMPP;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace AuctionSniper.Tests.Acceptance
{
    public class FakeAuctionServer
    {
        private readonly string _itemId;

        private readonly Connection _connection;

        private Chat _currentChat;

        public const string ItemIdAsLogin = "auction-{0}";

        public const string XmppHostName = "broker_channel";

        readonly SingleMessageListener _messageListener = new SingleMessageListener();

        public FakeAuctionServer(string itemId)
        {
            _itemId = itemId;
            _connection = new Connection(XmppHostName, string.Format(ItemIdAsLogin, itemId));
        }
        
        public string ItemId
        {
            get { return _itemId; }
        }

        public void StartSellingItem()
        {
            _connection.Connect();
            _connection.GetChatManager().AddChatListener(chat =>
            {
                _currentChat = chat;
                _currentChat.AddMessageListener(_messageListener.ProcessMessage);
            });
        }

        public void HasReceivedJoinRequestFrom(string sniperId)
        {
            ReceivesAMessageMatching(sniperId, Is.EqualTo(ApplicationConstants.JoinCommandFormat));
        }

        public void AnnounceClosed()
        {
            _currentChat.SendMessage(ApplicationConstants.CloseFormat);
        }

        public void Stop()
        {
            _connection.Disconnect();
        }

        public void ReportPrice(int price, int increment, string bidder)
        {
            _currentChat.SendMessage(string.Format("SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2}",
                price, increment, bidder));
        }

        public void HasReceivedBid(int bid, string sniperId)
        {
            var equalConstraint = Is.EqualTo(string.Format(ApplicationConstants.BidFormat, bid));
            ReceivesAMessageMatching(sniperId, equalConstraint);
        }

        private void ReceivesAMessageMatching(string sniperId, IResolveConstraint messageMatcher)
        {
            _messageListener.ReceivesAMessage(messageMatcher);
            Assert.That(_currentChat.Participant, Is.StringContaining(sniperId));
        }
    }
}
