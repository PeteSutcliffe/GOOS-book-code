using System;
using System.ComponentModel;
using System.Windows.Controls;
using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        public SniperWindow(SnipersTableModel model)
        {
            InitializeComponent();
            grid.AutoGeneratingColumn += GridOnAutoGeneratingColumn;
            grid.ItemsSource = model;
        }

        private void GridOnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = ((PropertyDescriptor)e.PropertyDescriptor).DisplayName;
        }
    }
}
