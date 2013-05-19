using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {        
        public SniperWindow()
        {
            InitializeComponent();
            SniperStatus.Text = ApplicationConstants.StatusJoining;

            grid.ItemsSource = new List<Customer>{new Customer { FirstName="Peter", LastName = "Sutcliffe"}};
        }

        public void ShowStatus(string status)
        {
            Dispatcher.BeginInvoke((Action)delegate { SniperStatus.Text = status; });
        }

        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
