namespace AuctionSniper.Domain
{
    public interface ISniperListener
    {
        void SniperStateChanged(SniperSnapshot sniperSnapshot);
    }
}