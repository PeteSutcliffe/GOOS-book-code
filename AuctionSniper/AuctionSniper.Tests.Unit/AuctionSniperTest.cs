using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionSniper.UI.Wpf;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    [TestFixture]
    public class AuctionSniperTest
    {
        private Sniper _sniper;
        private Mock<ISniperListener> _mockSniperListener;
        private Mock<IAuction> _mockAuction;

        [SetUp]
        public void Setup()
        {
            _mockSniperListener = new Mock<ISniperListener>();
            _mockAuction = new Mock<IAuction>();
            _sniper = new UI.Wpf.Sniper(_mockAuction.Object, _mockSniperListener.Object);
        }

        [Test]
        public void ReportsLostWhenAuctonLoses()
        {
            _sniper.AuctionClosed();
            _mockSniperListener.Verify(v => v.SniperLost(), Times.AtLeastOnce());
        }

        [Test]
        public void BidsHigherAndReportsBiddingWhenNewPriceArrives()
        {
            const int price = 1001;
            const int increment = 25;

            _sniper.CurrentPrice(price, increment);
            
            _mockAuction.Verify(a => a.Bid(price + increment), Times.Once());
            _mockSniperListener.Verify(l => l.SniperBidding(), Times.AtLeastOnce());
        }
    }
}
