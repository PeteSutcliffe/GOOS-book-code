using System.Diagnostics;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;

namespace AuctionSniper.Tests.Acceptance
{
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
            var window = _application.GetWindow("Auction Sniper Main");
            var label = window.Get<WPFLabel>("SniperStatus");
            Assert.That(label.Text, Is.EqualTo("Lost"));
        }
    }
}