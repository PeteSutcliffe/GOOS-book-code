namespace AuctionSniper.Domain
{
    public class Sniper : IAuctionEventListener
    {
        private readonly IAuction _auction;
        private readonly ISniperListener _sniperListener;
        private SniperSnapshot _snapshot;

        public Sniper(IAuction auction, ISniperListener sniperListener, string itemId)
        {
            _auction = auction;
            _sniperListener = sniperListener;
            _snapshot = SniperSnapshot.Joining(itemId);
            NotifyChanged();
        }

        public void AuctionClosed()
        {
            _snapshot = _snapshot.Closed();
            NotifyChanged();
        }

        public void CurrentPrice(int price, int increment, PriceSource priceSource)
        {
            switch(priceSource)
            {
                case PriceSource.FromSniper:
                    _snapshot = _snapshot.Winning(price);
                    break;
                case PriceSource.FromOtherBidder:
                    int bid = price + increment;
                    _auction.Bid(bid);
                    _snapshot = _snapshot.Bidding(price, bid);
                    break;
            }                    

            NotifyChanged();
        }

        private void NotifyChanged()
        {
            _sniperListener.SniperStateChanged(_snapshot);
        }
    }
}