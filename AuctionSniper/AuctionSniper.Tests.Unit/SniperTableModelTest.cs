using AuctionSniper.Domain;
using NUnit.Framework;
using System;

namespace AuctionSniper.Tests.Unit
{
    [TestFixture]
    public class SniperTableModelTest
    {
        private SnipersTableModel _model;

        [SetUp]
        public void Setup()
        {
            _model = new SnipersTableModel();
        }

        [Test]
        public void RaisesChangedEventWhenNewValuesSet()
        {
            bool changed = false;
            _model.CollectionChanged += (sender, args) => changed = true;

            _model.SniperStatusChanged(new SniperSnapshot("item id", 555, 666, SniperState.Bidding));

            Assert.That(changed);
        }
    }
}