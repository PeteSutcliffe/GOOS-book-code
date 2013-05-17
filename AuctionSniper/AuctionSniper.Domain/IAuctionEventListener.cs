namespace AuctionSniper.Domain
{
    public interface IAuctionEventListener
    {
        void AuctionClosed();
        void CurrentPrice(int price, int increment, PriceSource bidder);
    }

    public enum PriceSource
    {
        FromSniper, 
        FromOtherBidder
    }
}