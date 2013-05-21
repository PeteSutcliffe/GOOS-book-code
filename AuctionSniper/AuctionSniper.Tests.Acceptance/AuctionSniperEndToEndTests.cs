using NUnit.Framework;
using White.Core;

namespace AuctionSniper.Tests.Acceptance
{
    public class AuctionSniperEndToEndTests
    {
        readonly FakeAuctionServer _auction = new FakeAuctionServer("item-54321");
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
            _application.ShowsSniperHasLostAuction();
        }

        [Test]
        public void SniperMakesAHigherBidButLoses()
        {
            _auction.StartSellingItem();
            _application.StartBiddingIn(_auction);
            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding();

            _auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppId);

            _auction.AnnounceClosed();
            _application.ShowsSniperHasLostAuction();
        }

        [Test]
        public void SniperWinsAuctionByBiddingHigher()
        {
            _auction.StartSellingItem();
            _application.StartBiddingIn(_auction);
            _auction.HasReceivedJoinRequestFrom(ApplicationRunner.SniperXmppId);

            _auction.ReportPrice(1000, 98, "other bidder");
            _application.HasShownSniperIsBidding(1000, 1098);

            _auction.HasReceivedBid(1098, ApplicationRunner.SniperXmppId);
            _auction.ReportPrice(1098, 97, ApplicationRunner.SniperXmppId);
            _application.HasShownSniperIsWinning(1098);

            _auction.AnnounceClosed();
            _application.ShowsSniperHasWonAuction(1098);
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
