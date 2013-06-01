using System;
using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class AppMain
    {
        private readonly Connection _connection;
        private readonly SniperWindow _ui;

        private const int ArgHostName = 0;
        private const int ArgUserName = 1;
        private const string ItemIdAsLogin = "auction-{0}";

        readonly SnipersTableModel _snipers = new SnipersTableModel();

        public static void Run(string[] args)
        {                                    
            var hostName = args[ArgHostName];
            var userName = args[ArgUserName];

            var connection = ConnectTo(hostName, userName);
            var appMain = new AppMain(connection);
            for (var i = 2; i < args.Length; i++)
            {
                appMain.JoinAuction(args[i]);
            }
        }

        public AppMain(Connection connection)
        {
            _connection = connection;
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

        private void JoinAuction(string itemId)
        {
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