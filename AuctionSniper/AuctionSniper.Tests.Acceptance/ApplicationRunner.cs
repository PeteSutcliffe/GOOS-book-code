using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using AuctionSniper.Domain;

using NUnit.Framework;
using White.Core;
using White.Core.UIItems;
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
            _application.WaitWhileBusy();
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
            HasCellWithText(status);
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

        private void HasRowWithMatchingCells(params string[] values)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var grid = window.Get<ListView>("grid");

            bool foundRow = false;

            foreach (var row in grid.Rows)
            {
                var cells = row.GetCells(grid.Header);

                bool matchedAll = true;

                for (int i = 0; i < cells.Count ; i++)
                {
                    if (cells[i].Text != values[0])
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

            Assert.That(foundRow, Is.EqualTo(true), "Row matching supplied values not found");            
        }

        public void HasShownSniperIsBidding(int lastPrice, int lastBid)
        {
            HasRowWithMatchingCells(lastPrice.ToString(), lastBid.ToString());
        }

        public void HasShownSniperIsWinning(int winningBid)
        {
            HasRowWithMatchingCells(winningBid.ToString());
        }

        public void ShowsSniperHasWonAuction(int lastPrice)
        {
            HasRowWithMatchingCells(lastPrice.ToString());
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