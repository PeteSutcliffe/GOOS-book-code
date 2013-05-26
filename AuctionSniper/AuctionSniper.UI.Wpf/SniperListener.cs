using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    internal class SniperListener : ISniperListener
    {
        private readonly SnipersTableModel _model;

        public SniperListener(SnipersTableModel model)
        {
            _model = model;
        }

        public void SniperStateChanged(SniperSnapshot sniperSnapshot)
        {
            _model.SniperStatusChanged(sniperSnapshot);
        }
    }
}