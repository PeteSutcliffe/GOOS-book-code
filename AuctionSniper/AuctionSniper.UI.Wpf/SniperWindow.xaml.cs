using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        public SniperWindow(SnipersTableModel model)
        {
            InitializeComponent();

            grid.ItemsSource = model.DefaultView;
        }
    }
}
