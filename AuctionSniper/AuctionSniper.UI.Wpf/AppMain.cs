using System;
using System.IO;

using AuctionSniper.XMPP;

namespace AuctionSniper.UI.Wpf
{
    public class AppMain : ISniperListener
    {
        private readonly SniperWindow _ui;

        private Connection _connection;

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

            chat.AddMessageListener(
                    new AuctionMessageTranslator(new Sniper(new NullAuction(chat), this)).ProcessMessage
                );

            chat.SendMessage(new Message(ApplicationConstants.JoinCommandFormat));
        }

        public void SniperLost()
        {
            _ui.ShowStatus(ApplicationConstants.StatusLost);
        }

        public void SniperBidding()
        {
            _ui.ShowStatus(ApplicationConstants.StatusBidding);
        }
    }

    public class NullAuction : IAuction
    {
        private readonly Chat _chat;

        public NullAuction(Chat chat)
        {
            _chat = chat;
        }

        public void Bid(int amount)
        {
            try
            {
                _chat.SendMessage(string.Format(ApplicationConstants.BidFormat, amount));
            }
            catch (Exception)
            {
                //todo: how to show error?
                throw;
            }
        }
    }
}