namespace AuctionSniper.Domain
{
    public class Sniper : IAuctionEventListener
    {
        private readonly IAuction _auction;
        private readonly ISniperListener _sniperListener;
        private bool _isWinning;

        public Sniper(IAuction auction, ISniperListener sniperListener)
        {
            _auction = auction;
            _sniperListener = sniperListener;
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
                _auction.Bid(price + increment);
                _sniperListener.SniperBidding();
            }                    
        }
    }
}