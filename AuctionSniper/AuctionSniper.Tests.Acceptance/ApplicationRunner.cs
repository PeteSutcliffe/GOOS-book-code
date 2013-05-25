using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using AuctionSniper.Domain;

using NUnit.Framework;
using White.Core;
using White.Core.UIItems;
using White.Core.UIItems.ListViewItems;

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
            _driver.ShowsSniperStatus(status);
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

    class AuctionSniperDriver
    {
        private readonly Application _application;

        public AuctionSniperDriver(Application application)
        {
            _application = application;
        }

        public void ShowsSniperStatus(string itemId, int lastPrice, int lastbid, string status)
        {
            HasRowWithMatchingCells(itemId, lastPrice, lastbid, status);
        }

        private void HasCellWithText(string text)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var grid = window.Get<ListView>("grid");

            bool foundText = false;

            foreach (var row in grid.Rows)
            {
                if (row.GetCells(grid.Header).Any(cell => cell.Text == text))
                {
                    foundText = true;
                }
                if (foundText) break;
            }

            Assert.That(foundText, Is.EqualTo(true), string.Format("Cell with text '{0}' not found", text));
        }

        private void HasRowWithMatchingCells(params object[] values)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var grid = window.Get<ListView>("grid");

            bool foundRow = false;

            foreach (var row in grid.Rows)
            {
                var cells = row.GetCells(grid.Header);

                bool matchedAll = true;

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i].Text != values[i].ToString())
                    {
                        matchedAll = false;
                        break;
                    }
                }

                if (matchedAll)
                {
                    foundRow = true;
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Could not find row with values: ");
            for (int i = 0; i < values.Length; i++)
            {
                sb.AppendFormat("'{0}'", values[i]);
                if (i != values.Length - 1)
                    sb.Append(", ");
            }

                Assert.That(foundRow, Is.EqualTo(true), sb.ToString());
        }

        public void ShowsSniperStatus(string itemId)
        {
            HasCellWithText(itemId);
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