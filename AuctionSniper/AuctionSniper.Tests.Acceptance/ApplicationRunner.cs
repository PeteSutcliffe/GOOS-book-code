using System.Diagnostics;
using AuctionSniper.Domain;
using White.Core;

namespace AuctionSniper.Tests.Acceptance
{
    class ApplicationRunner
    {
        private Application _application;

        private string _itemId;

        private AuctionSniperDriver _driver;

        public const string SniperXmppId = "sniper";

        public void StartBiddingIn(FakeAuctionServer auction)
        {
            _application = Application.Launch(
                new ProcessStartInfo("AuctionSniper.UI.Wpf.exe",
                                     string.Format("broker_channel {0} {1}", SniperXmppId, auction.ItemId)));
            _application.WaitWhileBusy();
            _itemId = auction.ItemId;
            _driver = new AuctionSniperDriver(_application);
        }

        public void Stop()
        {
            _application.Close();
        }

        public void ShowsSniperHasLostAuction(int lastPrice, int lastBid)
        {
            _driver.ShowsSniperStatus(_itemId, lastPrice, lastBid, ApplicationConstants.StatusLost);
        }

        public void HasShownSniperIsBidding(int lastPrice, int lastBid)
        {
            _driver.ShowsSniperStatus(_itemId, lastPrice, lastBid, ApplicationConstants.StatusBidding);
        }

        public void HasShownSniperIsWinning(int winningBid)
        {
            _driver.ShowsSniperStatus(_itemId, winningBid, winningBid, ApplicationConstants.StatusWinning);
        }

        public void ShowsSniperHasWonAuction(int lastPrice)
        {
            _driver.ShowsSniperStatus(_itemId, lastPrice, lastPrice, ApplicationConstants.StatusWon);
        }
    }
}