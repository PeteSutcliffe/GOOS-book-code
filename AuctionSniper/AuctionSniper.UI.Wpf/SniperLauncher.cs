using System.Collections.Generic;
using System.Windows.Threading;
using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class SniperLauncher
    {
        private readonly XMPPAuctionHouse _auctionHouse;
        private readonly SnipersTableModel _snipers;
        private readonly Dispatcher _dispatcher;
        private readonly List<XMPPAuction> _auctions = new List<XMPPAuction>();


        public SniperLauncher(XMPPAuctionHouse auctionHouse, SnipersTableModel snipers, Dispatcher dispatcher)
        {
            _auctionHouse = auctionHouse;
            _snipers = snipers;
            _dispatcher = dispatcher;
        }

        public void JoinAuction(string itemId)
        {
            _snipers.AddSniper(SniperSnapshot.Joining(itemId));
            var auction = _auctionHouse.AuctionFor(itemId);
            _auctions.Add(auction);
            var sniper = new Sniper(auction, new SniperListener(_dispatcher, _snipers), itemId);
            auction.AddAuctionEventListener(sniper);
            auction.Join();
        }
    }
}