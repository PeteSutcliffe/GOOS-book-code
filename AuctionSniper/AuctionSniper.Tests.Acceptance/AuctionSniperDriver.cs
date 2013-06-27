using System.Text;
using System.Threading;
using NUnit.Framework;
using White.Core;
using White.Core.UIItems;
using White.Core.UIItems.WindowItems;

namespace AuctionSniper.Tests.Acceptance
{
    class AuctionSniperDriver
    {
        private readonly Application _application;
        private Window _window;
        private ListView _grid;

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
            FindGrid();
            Assert.That(_grid.Header.Columns[0].Text, Is.EqualTo("Item"));
            Assert.That(_grid.Header.Columns[1].Text, Is.EqualTo("Last Price"));
            Assert.That(_grid.Header.Columns[2].Text, Is.EqualTo("Last Bid"));
            Assert.That(_grid.Header.Columns[3].Text, Is.EqualTo("State"));
        }

        private void HasRowWithMatchingCells(params object[] values)
        {
            _application.WaitWhileBusy();
            FindGrid();

            bool foundRow = false;

            foreach (var row in _grid.Rows)
            {
                var cells = row.GetCells(_grid.Header);

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


        private void FindGrid()
        {
            if (_window == null || _grid == null)
            {
                _application.WaitWhileBusy();
                Thread.Sleep(500);
                _window = _application.GetWindow("Auction Sniper Main");
                _grid = _window.Get<ListView>("grid");
            }
        }

        public void StartBiddingFor(string itemId, int stopPrice)
        {
            var window = _application.GetWindow("Auction Sniper Main");
            var textField = window.Get<TextBox>("ItemId");
            textField.SetValue(itemId);

            var stopField = window.Get<TextBox>("StopPrice");
            stopField.SetValue(stopPrice);

            var button = window.Get<Button>("JoinAuction");
            button.Click();
        }
    }
}