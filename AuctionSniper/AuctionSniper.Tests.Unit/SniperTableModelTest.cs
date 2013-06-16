using AuctionSniper.Domain;
using AuctionSniper.UI.Wpf;
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
            _model = new SnipersTableModel(null);
        }

        [Test]
        public void RaisesChangedEventWhenNewValuesSet()
        {
            bool changed = false;
            
            var joining = SniperSnapshot.Joining("item123");
            _model.AddSniperSnapshot(joining);
            _model.CollectionChanged += (sender, args) => changed = true;

            _model.SniperStatusChanged(new SniperSnapshot("item123", 555, 666, SniperState.Bidding));

            Assert.That(changed);
            Assert.That(_model.Count == 1);
            Assert.That(_model[0].ItemId, Is.EqualTo("item123"));
            Assert.That(_model[0].LastBid, Is.EqualTo(666));
            Assert.That(_model[0].LastPrice, Is.EqualTo(555));
            Assert.That(_model[0].State, Is.EqualTo(SnipersTableModel.TableItem.TextFor(SniperState.Bidding)));
        }

        [Test]
        public void NotifiesListenersWhenAddingASniper()
        {
            bool changed = false;            

            _model.CollectionChanged += (sender, args) => changed = true;
            
            var joining = SniperSnapshot.Joining("item123");
            _model.AddSniperSnapshot(joining);

            Assert.That(changed);
            Assert.That(_model.Count == 1);
            Assert.That(_model[0].ItemId, Is.EqualTo("item123"));
            Assert.That(_model[0].LastBid, Is.EqualTo(0));
            Assert.That(_model[0].LastPrice, Is.EqualTo(0));
            Assert.That(_model[0].State, Is.EqualTo(SnipersTableModel.TableItem.TextFor(SniperState.Joining)));
        }
    }
}