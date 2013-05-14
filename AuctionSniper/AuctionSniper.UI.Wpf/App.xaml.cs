using System.Windows;

namespace AuctionSniper.UI.Wpf
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppMain.Run(e.Args);
        }
    }
}
