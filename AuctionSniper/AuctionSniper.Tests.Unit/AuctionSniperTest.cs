using AuctionSniper.Domain;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    [TestFixture]
    public class AuctionSniperTest
    {
        private Sniper _sniper;
        private MockWithState<ISniperListener> _mockSniperListener;
        private Mock<IAuction> _mockAuction;
        private const string ItemId = "ITEM";
        private const int StopPrice = 1234;

        [SetUp]
        public void Setup()
        {
            _mockSniperListener = new MockWithState<ISniperListener>();
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
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                               "bidding");

            _mockSniperListener.RestrictState(
                l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)),
                "bidding");

            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.AuctionClosed();

            _mockSniperListener.VerifyRestrictedActions();
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
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                "bidding");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                "bidding");

            _sniper.CurrentPrice(123, 12, PriceSource.FromOtherBidder);
            _sniper.CurrentPrice(135, 45, PriceSource.FromSniper);

            _mockSniperListener.VerifyRestrictedActions();
        }

        [Test]
        public void ReportsWonIfAuctionClosesWhenWinning()
        {
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                "winning");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Won)),
                               "winning");

            _sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            _sniper.AuctionClosed();

            _mockSniperListener.VerifyRestrictedActions();
        }

        [Test]
        public void DoesNotBidAndReportsLosingIfSubsequentPriceIsAboveStopPrice()
        {
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                "bidding");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                "bidding");

            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.VerifyRestrictedActions();
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
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                "losing");

            _mockSniperListener.RestrictState(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Lost)),
                "losing");

            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _sniper.AuctionClosed();

            _mockSniperListener.VerifyRestrictedActions();
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
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Winning)),
                "winning");

            _mockSniperListener.RestrictState(
                l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Losing)),
                "winning");

            _sniper.CurrentPrice(234, 45, PriceSource.FromSniper);

            _sniper.CurrentPrice(2345, 25, PriceSource.FromOtherBidder);

            _mockSniperListener.VerifyRestrictedActions();
        }

        [Test]
        public void ReportsFailedIfAuctionFailsWhenBidding()
        {
            _mockSniperListener.Allow(l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Bidding)),
                "bidding");

            _mockSniperListener.RestrictState(
                l => l.SniperStateChanged(It.Is<SniperSnapshot>(s => s.State == SniperState.Failed)),
                "bidding");

            _sniper.CurrentPrice(123, 45, PriceSource.FromOtherBidder);
            _sniper.AuctionFailed();

            _mockSniperListener.VerifyRestrictedActions();
        }
    }
}
