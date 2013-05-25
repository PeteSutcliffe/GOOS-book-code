using AuctionSniper.Domain;
using NUnit.Framework;
using System;

namespace AuctionSniper.Tests.Unit
{
    public class SniperTableModelTest
    {
        private SnipersTableModel _model;

        [SetUp]
        public void Setup()
        {
            _model = new SnipersTableModel();            
        }

        [Test]
        public void HasEnoughColumns()
        {
            Assert.That(_model.Columns.Count, Is.EqualTo(Enum.GetValues(typeof(SnipersTableModel.Column)).Length));
        }

        [Test]
        public void SetsSniperValuesInColumns()
        {
            bool changed = false;
            _model.RowChanged += (sender, args) => changed = true;

            _model.SniperStatusChanged(new Sniperstate("item id", 555, 666), ApplicationConstants.StatusBidding);

            AssertColumnEquals(SnipersTableModel.Column.ItemIdentifier, "item id");
            AssertColumnEquals(SnipersTableModel.Column.LastPrice, 555);
            AssertColumnEquals(SnipersTableModel.Column.LastBid, 666);
            AssertColumnEquals(SnipersTableModel.Column.SniperStatus, ApplicationConstants.StatusBidding);

            Assert.That(changed);
        }

        private void AssertColumnEquals(SnipersTableModel.Column column, object expected)
        {
            const int rowIndex = 0;
            int columnIndex = (int) column;
            Assert.AreEqual(expected, _model.Rows[rowIndex][columnIndex]);
        }
    }
}