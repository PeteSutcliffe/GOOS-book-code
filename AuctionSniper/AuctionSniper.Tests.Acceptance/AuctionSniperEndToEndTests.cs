using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;

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

    internal class ApplicationRunner
    {
        private Application _application;
        

        public void StartBiddingIn(FakeAuctionServer auction)
        {
            _application = Application.Launch(
                new ProcessStartInfo("AuctionSniper.UI.Wpf.exe",
                                     string.Format("broker_channel sniper {0}", auction.ItemId)));
        }

        public void Stop()
        {
            _application.Close();
        }

        public void ShowsSniperHasLostAuction()
        {
            var window = _application.GetWindow("MainWindow");
            var label = window.Get<WPFLabel>("SniperStatus");
            Assert.That(label.Text, Is.EqualTo("Lost"));
        }
    }
}
