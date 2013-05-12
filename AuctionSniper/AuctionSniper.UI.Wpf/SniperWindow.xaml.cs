using System;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SniperWindow : IAuctionEventListener
    {
        private const int ArgHostName = 0;
        private const int ArgUserName = 1;
        private const int ArgItemId = 2;

        private const string ItemIdAsLogin = "auction-{0}";

        public static void Start(string[] args)
        {
            var window = new SniperWindow();
            window.Show();
            var hostName = args[ArgHostName];
            var userName = args[ArgUserName];
            var itemId = args[ArgItemId];

            window.JoinAuction(hostName, userName, itemId);
        }

        private void JoinAuction(string hostName, string userName, string itemId)
        {
            var connection = ConnectTo(hostName, userName);

            var chat = connection.GetChatManager().CreateChat(
                AuctionId(itemId), new AuctionMessageTranslator(this).ProcessMessage
                );
            chat.SendMessage(new Message(ApplicationConstants.JoinCommandFormat));
        }

        public void AuctionClosed()
        {
            Dispatcher.BeginInvoke((Action)delegate { SniperStatus.Text = ApplicationConstants.StatusLost; });
        }

        public void CurrentPrice(int price, int increment)
        {
            
        }

        private static Connection ConnectTo(string hostName, string userName)
        {
            var connection = new Connection(hostName, userName);
            connection.Connect();
            return connection;
        }

        private static string AuctionId(string itemId)
        {
            return string.Format(ItemIdAsLogin, itemId);
        }

        private SniperWindow()
        {
            InitializeComponent();
            SniperStatus.Text = ApplicationConstants.StatusJoining;
        }
    }
}
