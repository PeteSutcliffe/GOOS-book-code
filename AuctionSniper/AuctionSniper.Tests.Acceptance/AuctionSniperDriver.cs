using System.Text;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;

namespace AuctionSniper.Tests.Acceptance
{
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

        public void HasColumnTitles()
        {
            var grid = FindGrid();
            Assert.That(grid.Header.Columns[0].Text, Is.EqualTo("Item"));
            Assert.That(grid.Header.Columns[1].Text, Is.EqualTo("Last Price"));
            Assert.That(grid.Header.Columns[2].Text, Is.EqualTo("Last Bid"));
            Assert.That(grid.Header.Columns[3].Text, Is.EqualTo("State"));
        }

        private void HasRowWithMatchingCells(params object[] values)
        {
            var grid = FindGrid();

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


        private ListView FindGrid()
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var grid = window.Get<ListView>("grid");
            return grid;
        }

        public void StartBiddingFor(string itemId)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var textField = window.Get<TextBox>("ItemId");
            textField.SetValue(itemId);
            var button = window.Get<Button>("JoinAuction");
            button.Click();
        }
    }
}