using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class SniperLauncher
    {
        private readonly XMPPAuctionHouse _auctionHouse;

        private readonly ISniperCollector _collector;

        public SniperLauncher(XMPPAuctionHouse auctionHouse, ISniperCollector snipers)
        {
            _auctionHouse = auctionHouse;
            _collector = snipers;
        }

        public void JoinAuction(string itemId)
        {
            var auction = _auctionHouse.AuctionFor(itemId);

            var sniper = new Sniper(auction, itemId);
            auction.AddAuctionEventListener(sniper);
            _collector.AddSniper(sniper);

            auction.Join();
        }
    }
}