namespace AuctionSniper.Domain
{
    public class Sniper : IAuctionEventListener
    {
        private readonly IAuction _auction;
        private readonly ISniperListener _sniperListener;
        private readonly string _itemId;
        private bool _isWinning;

        public Sniper(IAuction auction, ISniperListener sniperListener, string itemId)
        {
            _auction = auction;
            _sniperListener = sniperListener;
            _itemId = itemId;
        }

        public void AuctionClosed()
        {
            if (_isWinning)
            {
                _sniperListener.SniperWon();
            }
            else
            {
                _sniperListener.SniperLost();
            }            
        }

        public void CurrentPrice(int price, int increment, PriceSource priceSource)
        {
            _isWinning = priceSource == PriceSource.FromSniper;

            if (_isWinning)
            {
                _sniperListener.SniperWinning();
            }
            else
            {
                int bid = price + increment;

                _auction.Bid(bid);
                _sniperListener.SniperBidding(new Sniperstate(_itemId, price, bid));
            }                    
        }
    }
}