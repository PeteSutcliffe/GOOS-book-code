namespace AuctionSniper.Domain
{
    public interface IAuctionEventListener
    {
        void AuctionClosed();
        void CurrentPrice(int price, int increment, PriceSource bidder);
        void AuctionFailed();
    }

    public enum PriceSource
    {
        FromSniper, 
        FromOtherBidder
    }
}