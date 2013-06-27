using System.Diagnostics;
using System.Threading;
using AuctionSniper.Domain;
using AuctionSniper.UI.Wpf;
using White.Core;

namespace AuctionSniper.Tests.Acceptance
{
    class ApplicationRunner
    {
        private Application _application;

        private AuctionSniperDriver _driver;

        public const string SniperXmppId = "sniper";        

        public void StartBiddingIn(params FakeAuctionServer[] auctions)
        {
            StartApplication();

            foreach (var auction in auctions)
            {
                _driver.StartBiddingFor(auction.ItemId, int.MaxValue);
                _driver.ShowsSniperStatus(auction.ItemId, 0, 0, SnipersTableModel.TextFor(SniperState.Joining));                
            }
        }

        public void StartBiddingWithStopPrice(FakeAuctionServer auction, int stopPrice)
        {
            StartApplication();

            _driver.StartBiddingFor(auction.ItemId, stopPrice);
            _driver.ShowsSniperStatus(auction.ItemId, 0, 0, SnipersTableModel.TextFor(SniperState.Joining));
        }

        private void StartApplication()
        {
            _application = Application.Launch(
                new ProcessStartInfo("AuctionSniper.UI.Wpf.exe",
                                     Arguments()));
            _application.WaitWhileBusy();
            _driver = new AuctionSniperDriver(_application);
            _driver.HasColumnTitles();
        }

        private static string Arguments()
        {
            return string.Format("broker_channel {0}", SniperXmppId);
        }

        public void Stop()
        {
            _application.Close();
            _application.Dispose();
            Thread.Sleep(250);
        }

        public void ShowsSniperHasLostAuction(FakeAuctionServer auction, int lastPrice, int lastBid)
        {
            _driver.ShowsSniperStatus(auction.ItemId, lastPrice, lastBid, SnipersTableModel.TextFor(SniperState.Lost));
        }

        public void HasShownSniperIsBidding(FakeAuctionServer auction, int lastPrice, int lastBid)
        {
            _driver.ShowsSniperStatus(auction.ItemId, lastPrice, lastBid, SnipersTableModel.TextFor(SniperState.Bidding));
        }

        public void HasShownSniperIsWinning(FakeAuctionServer auction, int winningBid)
        {
            _driver.ShowsSniperStatus(auction.ItemId, winningBid, winningBid, SnipersTableModel.TextFor(SniperState.Winning));
        }

        public void ShowsSniperHasWonAuction(FakeAuctionServer auction, int lastPrice)
        {
            _driver.ShowsSniperStatus(auction.ItemId, lastPrice, lastPrice, SnipersTableModel.TextFor(SniperState.Won));
        }

        public void HasShownSniperIsLosing(FakeAuctionServer auction, int lastPrice, int lastBid)
        {
            _driver.ShowsSniperStatus(auction.ItemId, lastPrice, lastBid, SnipersTableModel.TextFor(SniperState.Losing));
        }

        public void ShowsSniperHasFailed(FakeAuctionServer auction)
        {
            _driver.ShowsSniperStatus(auction.ItemId, 0, 0, SnipersTableModel.TextFor(SniperState.Failed));
        }

        public void ReportsInvalidMessage(FakeAuctionServer auction, string aBrokenMessage)
        {
            //TODO...
        }
    }
}