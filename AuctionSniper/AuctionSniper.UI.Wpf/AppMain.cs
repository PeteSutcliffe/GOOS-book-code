using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class AppMain
    {
        private readonly SniperWindow _ui;

        private Connection _connection;

        private const int ArgHostName = 0;
        private const int ArgUserName = 1;
        private const int ArgItemId = 2;
        private const string ItemIdAsLogin = "auction-{0}";  
      
        SnipersTableModel _snipers = new SnipersTableModel();

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
            _ui = new SniperWindow(_snipers);
            _ui.Show();
            _ui.Closing += UiClosing;
        }

        void UiClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _connection.Disconnect();
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
            _connection = ConnectTo(hostName, userName);

            var chat = _connection.GetChatManager()
                                 .CreateChat(AuctionId(itemId), null);

            var auction = new XMPPAuction(chat);
            chat.AddMessageListener(
                new AuctionMessageTranslator(
                    _connection.User, 
                    new Sniper(auction, new SniperListener(_ui.Dispatcher, _snipers), itemId)).ProcessMessage);
            auction.Join();
        }
    }
}