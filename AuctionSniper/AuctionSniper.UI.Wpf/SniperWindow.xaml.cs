using System;
using System.ComponentModel;
using System.Windows.Controls;
using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        private Action<string> _action;

        public SniperWindow(SniperPortfolio portfolio)
        {
            InitializeComponent();

            grid.AutoGeneratingColumn += GridOnAutoGeneratingColumn;

            JoinAuction.Click += JoinAuction_Click;

            grid.ItemsSource = CreateTableModel(portfolio);
        }        

        public void SetUserRequestListener(Action<string> action)
        {
            _action = action;
        }

        private void JoinAuction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _action(ItemId.Text);
        }

        private void GridOnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
        }

        private SnipersTableModel CreateTableModel(SniperPortfolio portfolio)
        {
            var model = new SnipersTableModel(new WpfDispatcher(Dispatcher));
            portfolio.AddPortfolioListener(model);

            return model;
        }
    }
}
