namespace AuctionSniper.Domain
{
    public class Sniper : IAuctionEventListener
    {
        private readonly IAuction _auction;
        private readonly Item _item;
        private ISniperListener _sniperListener;
        private SniperSnapshot _snapshot;

        public Sniper(IAuction auction, Item item)
        {
            _auction = auction;
            _item = item;
            _snapshot = SniperSnapshot.Joining(item.Identifier);
        }

        public SniperSnapshot SniperSnapShot
        {
            get { return _snapshot; }
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
                    if (_item.AllowsBid(bid))
                    {
                        _auction.Bid(bid);
                        _snapshot = _snapshot.Bidding(price, bid);
                    }
                    else
                    {
                        _snapshot = _snapshot.Losing(price);
                    }
                    break;
            }                    

            NotifyChanged();
        }

        public void AuctionFailed()
        {
            _snapshot = _snapshot.Failed();
            NotifyChanged();
        }

        public void AddSniperListener(ISniperListener sniperListener)
        {
            _sniperListener = sniperListener;
        }

        private void NotifyChanged()
        {
            _sniperListener.SniperStateChanged(_snapshot);
        }
    }
}