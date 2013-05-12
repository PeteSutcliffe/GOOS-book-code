using System.Threading;
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
            _auction.HasReceivedJoinRequestFromSniper();
            _auction.AnnounceClosed();
            _application.ShowsSniperHasLostAuction();
        }

        [TearDown]
        public void TearDown()
        {
            _auction.Stop();
            _application.Stop();
            _smppServer.Close();
        }
    }
}
