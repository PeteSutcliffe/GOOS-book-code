using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        private readonly SnipersTableModel _model;

        public SniperWindow()
        {
            InitializeComponent();

            _model = new SnipersTableModel();

            grid.ItemsSource = _model.DefaultView;
        }

        public void SniperStatusChanged(SniperSnapshot sniperSnapshot)
        {
            _model.SniperStatusChanged(sniperSnapshot);
        }
    }
}
