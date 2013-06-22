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
        private const string ItemId = "ITEM";
        private const int StopPrice = 1234;

        [SetUp]
        public void Setup()
        {
            _mockSniperListener = new Mock<ISniperListener>();
            _mockAuction = new Mock<IAuction>();
            _sniper = new Sniper(_mockAuction.Object, new Item(ItemId, StopPrice));
            _sniper.AddSniperListener(_mockSniperListener.Object);
        }

        [Test]
        public void ReportsLostIfAuctionClosesImmediately()
        {
            _sniper.AuctionClosed();
            _mockSniperListener.Verify(v => v.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)), 
                Times.AtLeastOnce());
        }

        [Test]
        public void ReportsLostIfAuctionClosesWhenBidding()
        {
            // An attempt to emulate the state tracking outlined in the book.

            string state = null;
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                               () => state = "bidding");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)),
                               () => Assert.That(state, Is.EqualTo("bidding")));

            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.AuctionClosed();

            _mockSniperListener.Verify(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)), 
                Times.AtLeastOnce());
        }

        [Test]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            const int price = 1001;
            const int increment = 25;
            const int bid = price + increment;

            _sniper.CurrentPrice(price, increment, PriceSource.FromOtherBidder);

            _mockAuction.Verify(a => a.Bid(bid), Times.Once());
            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, price, bid, SniperState.Bidding)), Times.AtLeastOnce());
        }

        [Test]
        public void ReportsIsWinningWhenCurrentPriceComesFromSniper()
        {
            string state = null;            

            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                () => state = "bidding");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                () => Assert.That(state, Is.EqualTo("bidding"), "SUT was not in the correct state"));

            _sniper.CurrentPrice(123, 12, PriceSource.FromOtherBidder);
            _sniper.CurrentPrice(135, 45, PriceSource.FromSniper);

            _mockSniperListener.Verify(l => 
                l.SniperStateChanged(new SniperSnapshot(ItemId, 135, 135, SniperState.Winning)), Times.AtLeastOnce());
        }

        [Test]
        public void ReportsWonIfAuctionClosesWhenWinning()
        {
            string state = null;

            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                () => state = "winning");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Won)),
                               () => Assert.That(state, Is.EqualTo("winning")));

            _sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            _sniper.AuctionClosed();

            _mockSniperListener.Verify(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Won)), 
                Times.AtLeastOnce());
        }

        [Test]
        public void DoesNotBidAndReportsLosingIfSubsequentPriceIsAboveStopPrice()
        {
            string state = null;

            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                () => state = "bidding");

            const int bid = 123 + 45;

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                () => Assert.That(state, Is.EqualTo("bidding"), "SUT was not in the correct state"));

            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2345, bid, SniperState.Losing)),
                Times.AtLeastOnce());
        }

        [Test]
        public void DoesNotBidAndReportsLosingIfFirstPriceIsAboveStopPrice()
        {            
            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.Verify(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)), Times.Never());
            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2345, 0, SniperState.Losing)),
                Times.AtLeastOnce());
        }

        [Test]
        public void ReportsLostIfAuctionClosesWhenLosing()
        {
            string state = null;

            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                () => state = "losing");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)),
                () => Assert.That(state, Is.EqualTo("losing"), "SUT was not in the correct state"));

            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _sniper.AuctionClosed();

            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2345, 0, SniperState.Lost)),
                Times.AtLeastOnce());
        }

        [Test]
        public void ContinuesToBeLosingOnceStopPriceHasBeenReached()
        {            
            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _sniper.CurrentPrice(2370, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2345, 0, SniperState.Losing)),
                Times.AtLeastOnce());

            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2370, 0, SniperState.Losing)),
                Times.AtLeastOnce());
        }

        [Test]
        public void DoesNoBidAndReportsLosingIfPriceAfterWinningIsAboveStopPrice()
        {
            string state = null;

            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                () => state = "winning");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                () => Assert.That(state, Is.EqualTo("winning"), "SUT was not in the correct state"));

            _sniper.CurrentPrice(234, 45, PriceSource.FromSniper);

            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.Verify(l => l.SniperStateChanged(new SniperSnapshot(ItemId, 2345, 0, SniperState.Losing)),
                Times.AtLeastOnce());
        }
    }
}
