using System.Diagnostics;
using AuctionSniper.Domain;
using AuctionSniper.UI.Wpf;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;

namespace AuctionSniper.Tests.Acceptance
{
    internal class ApplicationRunner
    {
        private Application _application;

        public const string SniperXmppId = "sniper";

        public void StartBiddingIn(FakeAuctionServer auction)
        {
            _application = Application.Launch(
                new ProcessStartInfo("AuctionSniper.UI.Wpf.exe",
                                     string.Format("broker_channel {0} {1}", SniperXmppId, auction.ItemId)));
        }

        public void Stop()
        {
            _application.Close();
        }

        public void ShowsSniperHasLostAuction()
        {
            HasShownSniperStatus(ApplicationConstants.StatusLost);
        }

        public void HasShownSniperIsBidding()
        {
            HasShownSniperStatus(ApplicationConstants.StatusBidding);
        }

        public void HasShownSniperIsWinning()
        {
            HasShownSniperStatus(ApplicationConstants.StatusWinning);
        }

        public void ShowsSniperHasWonAuction()
        {
            HasShownSniperStatus(ApplicationConstants.StatusWon);
        }        

        private void HasShownSniperStatus(string status)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var label = window.Get<WPFLabel>("SniperStatus");
            Assert.That(label.Text, Is.EqualTo(status));
        }
    }
}