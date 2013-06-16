﻿using System.Diagnostics;
using System.Linq;
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
            _application = Application.Launch(
                new ProcessStartInfo("AuctionSniper.UI.Wpf.exe",
                                     Arguments()));
            _application.WaitWhileBusy();
            _driver = new AuctionSniperDriver(_application);
            _driver.HasColumnTitles();

            foreach (var auction in auctions)
            {
                _driver.StartBiddingFor(auction.ItemId);
                _driver.ShowsSniperStatus(auction.ItemId, 0, 0, SnipersTableModel.TextFor(SniperState.Joining));                
            }
        }

        private static string Arguments()
        {
            return string.Format("broker_channel {0}", SniperXmppId);
        }

        public void Stop()
        {
            _application.Close();
            _application.Dispose();
            Thread.Sleep(100);
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
    }
}