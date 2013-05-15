using System;
using System.Windows;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {        
        public SniperWindow()
        {
            InitializeComponent();
            SniperStatus.Text = ApplicationConstants.StatusJoining;
        }

        public void ShowStatus(string status)
        {
            Dispatcher.BeginInvoke((Action)delegate { SniperStatus.Text = status; });
        }
    }
}
