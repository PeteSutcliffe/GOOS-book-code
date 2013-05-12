using AuctionSniper.UI.Wpf;
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

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<IAuctionEventListener>();
            _listener = _mock.Object;
            _translator = new AuctionMessageTranslator(_listener);
        }

        [Test]
        public void NotifiesAuctionClosedWhenCloseMessageReceived()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, new Message("SOLVersion: 1.1; Event:Close;"));

            //Assert
            _mock.Verify(m => m.AuctionClosed(), Times.Once());
        }

        [Test]
        public void NotifiesBidDetailsWhenCurrentPriceMessageReceived()
        {
            //Act
            _translator.ProcessMessage(_unusedChat, 
                new Message("SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: Someone else;"));

            //Assert
            _mock.Verify(m => m.CurrentPrice(192, 7), Times.Once());
        }
    }
}
