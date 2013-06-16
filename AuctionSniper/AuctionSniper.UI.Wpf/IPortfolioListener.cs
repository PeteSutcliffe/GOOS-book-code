using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public interface IPortfolioListener
    {
        void SniperAdded(Sniper sniper);
    }
}