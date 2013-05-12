using System;
using AuctionSniper.XMPP;

namespace AuctionSniper.Tests.Acceptance
{
    public class FakeAuctionServer
    {
        private readonly string _itemId;

        private readonly Connection _connection;

        private Chat _currentChat;

        public const string ItemIdAsLogin = "auction-{0}";

        public const string XmppHostName = "broker_channel";

        readonly SingleMessageListener _messageListener = new SingleMessageListener();

        public FakeAuctionServer(string itemId)
        {
            _itemId = itemId;
            _connection = new Connection(XmppHostName, string.Format(ItemIdAsLogin, itemId));
        }
        
        public object ItemId
        {
            get { return _itemId; }
        }

        public void StartSellingItem()
        {
            _connection.Connect();
            _connection.GetChatManager().AddChatListener(chat =>
            {
                _currentChat = chat;
                _currentChat.AddMessageListener(_messageListener.ProcessMessage);
            });
        }

        public void HasReceivedJoinRequestFromSniper()
        {
            _messageListener.ReceivesAMessage();
        }

        public void AnnounceClosed()
        {
            _currentChat.SendMessage(new Message());
        }

        public void Stop()
        {
            _connection.Discounnect();
        }
    }
}
