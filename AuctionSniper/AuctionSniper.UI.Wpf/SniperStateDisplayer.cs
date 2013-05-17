using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    internal class SniperStateDisplayer : ISniperListener
    {
        private readonly SniperWindow _ui;

        public SniperStateDisplayer(SniperWindow ui)
        {
            _ui = ui;
        }

        public void SniperLost()
        {
            ShowStatus(ApplicationConstants.StatusLost);
        }

        public void SniperBidding()
        {
            ShowStatus(ApplicationConstants.StatusBidding);
        }

        public void SniperWinning()
        {
            ShowStatus(ApplicationConstants.StatusWinning);
        }

        public void SniperWon()
        {
            ShowStatus(ApplicationConstants.StatusWon);
        }

        private void ShowStatus(string status)
        {
            _ui.ShowStatus(status);
        }
    }
}