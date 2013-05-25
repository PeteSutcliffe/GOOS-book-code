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
            _model.SetStatusText(ApplicationConstants.StatusJoining);

            grid.ItemsSource = _model.DefaultView;
        }

        public void SniperStatusChanged(string status)
        {
            _model.SetStatusText(status);
        }

        public void SniperStatusChanged(Sniperstate sniperstate, string status)
        {
            _model.SniperStatusChanged(sniperstate, status);
        }
    }
}
