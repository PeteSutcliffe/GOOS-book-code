using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    internal class SniperStateDisplayer : ISniperListener
    {
        private readonly SniperWindow _ui;

        public SniperStateDisplayer(SniperWindow ui)
        {
            _ui = ui;
        }

        public void SniperStateChanged(SniperSnapshot sniperSnapshot)
        {
            _ui.SniperStatusChanged(sniperSnapshot);
        }
    }
}