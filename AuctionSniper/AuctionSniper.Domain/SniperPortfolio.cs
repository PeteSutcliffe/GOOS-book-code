using System.Collections.Generic;

namespace AuctionSniper.Domain
{
    public class SniperPortfolio : ISniperCollector
    {
        private readonly List<Sniper> _snipers = new List<Sniper>();
        private IPortfolioListener _listener;

        public void AddSniper(Sniper sniper)
        {
            _snipers.Add(sniper);           
            _listener.SniperAdded(sniper);
        }

        public void AddPortfolioListener(IPortfolioListener listener)
        {
            _listener = listener;
        }
    }
}