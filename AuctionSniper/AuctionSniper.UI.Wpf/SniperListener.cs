using System.Windows.Threading;
using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    internal class SniperListener : ISniperListener
    {
        private readonly Dispatcher _dispatcher;
        private readonly SnipersTableModel _model;

        public SniperListener(Dispatcher dispatcher, SnipersTableModel model)
        {
            _dispatcher = dispatcher;
            _model = model;
        }

        public void SniperStateChanged(SniperSnapshot sniperSnapshot)
        {
            _dispatcher.Invoke(() => _model.SniperStatusChanged(sniperSnapshot));
        }
    }
}