using NUnit.Framework;
using White.Core;

namespace AuctionSniper.Tests.Acceptance
{
    public class AuctionSniperEndToEndTests
    {
        readonly FakeAuctionServer _auction = new FakeAuctionServer("item-54321");
        readonly FakeAuctionServer _auction2 = new FakeAuctionServer("item-65432");

        readonly ApplicationRunner _application = new ApplicationRunner();
        private Application _smppServer;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _smppServer = Application.Launch("AuctionSniper.MessageBroker.exe");            
        }

        [Test]
        public void SniperJoinsAuctionUntilAuctionCloses()
        {
            _auction.StartSellingItem();
            _application.StartBiddingIn(_auction);
            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);
            _auction.AnnounceClosed();
            _application.ShowsSniperHasLostAuction(_auction, 0, 0);
        }

        [Test]
        public void SniperMakesAHigherBidButLoses()
        {
            _auction.StartSellingItem();
            _application.StartBiddingIn(_auction);
            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(_auction, 1000, 1098);

            _auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppId);

            _auction.AnnounceClosed();
            _application.ShowsSniperHasLostAuction(_auction, 1000, 1098);
        }

        [Test]
        public void SniperWinsAuctionByBiddingHigher()
        {
            _auction.StartSellingItem();
            _application.StartBiddingIn(_auction);
            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(_auction, 1000, 1098);

            _auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppId);
            _auction.ReportPrice(1098, 97, ApplicationRunner.SniperXmppId);
            _application.HasShownSniperIsWinning(_auction, 1098);

            _auction.AnnounceClosed();
            _application.ShowsSniperHasWonAuction(_auction, 1098);
        }

        [Test]
        public void SniperBidsForMultipleItems()
        {
            _auction.StartSellingItem();
            _auction2.StartSellingItem();

            _application.StartBiddingIn(_auction, _auction2);

            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);
            _auction2.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1000, 98, "other bidder");
            _auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppId);
            
            _auction2.ReportPrice(500, 21, "other bidder");
            _auction2.HasReceivedBid(521, ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1098, 97, ApplicationRunner.SniperXmppId);
            _application.HasShownSniperIsWinning(_auction, 1098);
            
            _auction2.ReportPrice(521, 21, ApplicationRunner.SniperXmppId);
            _application.HasShownSniperIsWinning(_auction2, 521);

            _auction.AnnounceClosed();
            _auction2.AnnounceClosed();

            _application.ShowsSniperHasWonAuction(_auction, 1098);
            _application.ShowsSniperHasWonAuction(_auction2, 521);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _smppServer.Close();
        }

        [TearDown]
        public void TearDown()
        {
            _auction.Stop();
            _application.Stop();
        }
    }
}
