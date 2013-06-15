using System;
using AuctionSniper.Domain;
using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class AppMain
    {
        private readonly SniperWindow _ui;

        private const int ArgHostName = 0;
        private const int ArgUserName = 1;
        private const string ItemIdAsLogin = "auction-{0}";

        readonly SnipersTableModel _snipers = new SnipersTableModel();
        private Action _closeAction;

        public static void Run(string[] args)
        {                                    
            var hostName = args[ArgHostName];
            var userName = args[ArgUserName];

            var connection = ConnectTo(hostName, userName);
            var appMain = new AppMain(connection);
            appMain.OnUiClosing(connection.Disconnect);
        }

        private void OnUiClosing(Action closeAction)
        {
            _closeAction = closeAction;
        }

        public AppMain(Connection connection)
        {
            _ui = new SniperWindow(_snipers);
            _ui.Show();
            _ui.Closing += UiClosing;
            AddRequestListenerFor(connection);
        }

        void UiClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _closeAction.Invoke();
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

        private void AddRequestListenerFor(Connection connection)
        {
            _ui.SetUserRequestListener(itemId =>
                {
                    _snipers.AddSniper(SniperSnapshot.Joining(itemId));

                    var chat = connection.GetChatManager()
                                         .CreateChat(AuctionId(itemId), null);

                    var auction = new XMPPAuction(chat);
                    chat.AddMessageListener(
                        new AuctionMessageTranslator(
                            connection.User,
                            new Sniper(auction, new SniperListener(_ui.Dispatcher, _snipers), itemId)).ProcessMessage);
                    auction.Join();
                });
        }
    }
}