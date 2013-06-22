using System;
using System.ComponentModel;
using System.Windows.Controls;
using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        private Action<Item> _action;

        public SniperWindow(SniperPortfolio portfolio)
        {
            InitializeComponent();

            grid.AutoGeneratingColumn += GridOnAutoGeneratingColumn;

            JoinAuction.Click += JoinAuction_Click;

            grid.ItemsSource = CreateTableModel(portfolio);
        }        

        public void SetUserRequestListener(Action<Item> action)
        {
            _action = action;
        }

        private void JoinAuction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _action(new Item(ItemId.Text, int.Parse(StopPrice.Text)));
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
