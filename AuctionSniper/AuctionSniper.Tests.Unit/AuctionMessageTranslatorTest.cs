using AuctionSniper.Domain;
using AuctionSniper.XMPP;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    public class AuctionMessageTranslatorTest
    {
        readonly Chat _unusedChat = null;
        AuctionMessageTranslator _translator;
        private IAuctionEventListener _listener;
        private Mock<IAuctionEventListener> _mock;

        private const string SniperId = "sniper";

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IAuctionEventListener>();
            _listener = _mock.Object;
            _translator = new AuctionMessageTranslator(SniperId, _listener);
        }

        [Test]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, new Message("SOLVersion: 1.1; Event: CLOSE;"));

            //Assert
            _mock.Verify(m => m.AuctionClosed(), Times.Once());
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromOtherBidder()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, 
                new Message("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: Someone else;"));

            //Assert
            _mock.Verify(m => m.CurrentPrice(192, 7, PriceSource.FromOtherBidder), Times.Once());
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromSniper()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, 
                new Message(string.Format("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: {0};", SniperId)));

            //Assert
            _mock.Verify(m => m.CurrentPrice(192, 7, PriceSource.FromSniper), Times.Once());
        }
    }
}
