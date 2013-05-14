using System.Windows;

using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppMain.Run(e.Args);
        }
    }

    public class AppMain : IAuctionEventListener
    {
        private readonly SniperWindow _ui;

        private const int ArgHostName = 0;
        private const int ArgUserName = 1;
        private const int ArgItemId = 2;
        private const string ItemIdAsLogin = "auction-{0}";        

        public static void Run(string[] args)
        {            
            var appMain = new AppMain();

            var hostName = args[ArgHostName];
            var userName = args[ArgUserName];
            var itemId = args[ArgItemId];

            appMain.JoinAuction(hostName, userName, itemId);
        }

        public AppMain()
        {
            _ui = new SniperWindow();
            _ui.Show();
        }

        public void AuctionClosed()
        {
            _ui.ShowStatus(ApplicationConstants.StatusLost);
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

        private void JoinAuction(string hostName, string userName, string itemId)
        {
            var connection = ConnectTo(hostName, userName);

            var chat = connection.GetChatManager().CreateChat(AuctionId(itemId), new AuctionMessageTranslator(this).ProcessMessage);
            chat.SendMessage(new Message(ApplicationConstants.JoinCommandFormat));
        }
    }
}
