namespace AuctionSniper.UI.Wpf
{
    public interface IAuctionEventListener
    {
        void AuctionClosed();
        void CurrentPrice(int price, int increment);
    }

    public interface ISniperListener
    {
        void SniperLost();
        void SniperBidding();
    }
}