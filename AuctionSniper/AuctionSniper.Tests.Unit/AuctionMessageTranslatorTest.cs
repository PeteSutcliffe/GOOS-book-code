using AuctionSniper.Domain;
using AuctionSniper.XMPP;
using Infrastructure.XMPP;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    public class AuctionMessageTranslatorTest
    {
        AuctionMessageTranslator _translator;
        private IAuctionEventListener _listener;
        private Mock<IAuctionEventListener> _mockListener;
        private readonly Chat _unusedChat = null;

        private const string SniperId = "sniper";

        [SetUp]
        public void Setup()
        {
            _mockListener = new Mock<IAuctionEventListener>();
            _listener = _mockListener.Object;
            _translator = new AuctionMessageTranslator(SniperId, _listener);
        }

        [Test]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, new Message("SOLVersion: 1.1; Event: CLOSE;"));

            //Assert
            _mockListener.Verify(m => m.AuctionClosed(), Times.Once());
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromOtherBidder()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, new Message("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: Someone else;"));

            //Assert
            _mockListener.Verify(m => m.CurrentPrice(192, 7, PriceSource.FromOtherBidder), Times.Once());
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceivedFromSniper()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, new Message(string.Format("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: {0};", SniperId)));

            //Assert
            _mockListener.Verify(m => m.CurrentPrice(192, 7, PriceSource.FromSniper), Times.Once());
        }

        [Test]
        public void NotifiesAuctionFailedWhenBadMessageReceived()
        {
            var message = new Message("a bad message");
            _translator.ProcessMessage(_unusedChat, message);

            _mockListener.Verify(m => m.AuctionFailed(), Times.Once());
        }

        [Test]
        public void NotifiesAuctionFailedWhenEventTypeMissing()
        {
            var message = new Message(string.Format("SOLVersion: 1.1; CurrentPrice: 234; Increment: 5; Bidder: {0};", SniperId));
            _translator.ProcessMessage(_unusedChat, message);

            _mockListener.Verify(m => m.AuctionFailed(), Times.Once());
        }

        [Test]
        public void NotifiesAuctionFailedWhenValueMissing()
        {
            var message = new Message(string.Format("SOLVersion: 1.1; Event: PRICE; CurrentPrice:; Increment: 5; Bidder: {0};", SniperId));
            _translator.ProcessMessage(_unusedChat, message);

            _mockListener.Verify(m => m.AuctionFailed(), Times.Once());
        }
    }
}
