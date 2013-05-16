namespace AuctionSniper.Domain
{
    public class Sniper : IAuctionEventListener
    {
        private readonly IAuction _auction;
        private readonly ISniperListener _sniperListener;

        public Sniper(IAuction auction, ISniperListener sniperListener)
        {
            _auction = auction;
            _sniperListener = sniperListener;
        }

        public void AuctionClosed()
        {
            _sniperListener.SniperLost();
        }

        public void CurrentPrice(int price, int increment)
        {
            _auction.Bid(price + increment);
            _sniperListener.SniperBidding();
        }
    }
}