using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using AuctionSniper.Domain;
using AuctionSniper.UI.Wpf;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListViewItems;

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
            var dataGird = window.Get<ListView>("grid");

            ListViewCells cells = dataGird.Rows[1].GetCells(dataGird.Header);

            ListViewCell cell = cells[0];

            Assert.That(label.Text, Is.EqualTo(status));
        }
    }

    public static class Extensions
    {
        public static ListViewCells GetCells(this ListViewRow row, ListViewHeader header)
        {

            AutomationElementCollection coll = row.AutomationElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridCell"));

            List<AutomationElement> list = coll.Cast<object>().Cast<AutomationElement>().ToList();

            return new ListViewCells(list, row.ActionListener, header);

        }
    }
}