namespace AuctionSniper.Domain
{
    public interface ISniperListener
    {
        void SniperLost();
        void SniperBidding(Sniperstate sniperstate);
        void SniperWinning();
        void SniperWon();
    }
}