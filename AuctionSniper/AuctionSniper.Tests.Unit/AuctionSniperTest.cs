using AuctionSniper.Domain;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    [TestFixture]
    public class AuctionSniperTest
    {
        private Sniper _sniper;
        private Mock<ISniperListener> _mockSniperListener;
        private Mock<IAuction> _mockAuction;

        [SetUp]
        public void Setup()
        {
            _mockSniperListener = new Mock<ISniperListener>();
            _mockAuction = new Mock<IAuction>();
            _sniper = new Sniper(_mockAuction.Object, _mockSniperListener.Object);
        }

        [Test]
        public void ReportsLostIfAuctionClosesImmediately()
        {
            _sniper.AuctionClosed();
            _mockSniperListener.Verify(v => v.SniperLost(), Times.AtLeastOnce());
        }

        [Test]
        public void ReportsLostIfAuctionClosesWhenBidding()
        {
            // An attempt to emulate the state tracking outlined in the book.

            string state = null;
            _mockSniperListener.Setup(l => l.SniperBidding())
                               .Callback(() => state = "bidding");

            _mockSniperListener.Setup(l => l.SniperLost())
                               .Callback(() => 
                                   Assert.That(state, Is.EqualTo("bidding")));
            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.AuctionClosed();

            _mockSniperListener.Verify(l => l.SniperLost(), Times.AtLeastOnce());
        }

        [Test]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            const int price = 1001;
            const int increment = 25;

            _sniper.CurrentPrice(price, increment, PriceSource.FromOtherBidder);
            
            _mockAuction.Verify(a => a.Bid(price + increment), Times.Once());
            _mockSniperListener.Verify(l => l.SniperBidding(), Times.AtLeastOnce());
        }

        [Test]
        public void ReportsIsWinningWhenCurrentPriceComesFromSniper()
        {
            _sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            _mockSniperListener.Verify(l => l.SniperWinning(), Times.AtLeastOnce());
        }

        [Test]
        public void ReportsWonIfAuctionClosesWhenWinning()
        {
            string state = null;

            _mockSniperListener.Setup(l => l.SniperWinning())
                               .Callback(() => state = "winning");
            _mockSniperListener.Setup(l => l.SniperWon())
                               .Callback(() =>
                                         Assert.That(state, Is.EqualTo("winning")));

            _sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            _sniper.AuctionClosed();

            _mockSniperListener.Verify(l => l.SniperWon(), Times.AtLeastOnce());
        }
    }
}
