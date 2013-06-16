using AuctionSniper.Domain;

namespace AuctionSniper.XMPP
{
    public class XMPPAuction : IAuction
    {
        private readonly Connection _connection;
        private readonly Chat _chat;

        private const string ItemIdAsLogin = "auction-{0}";

        public XMPPAuction(Connection connection, string itemId)
        {
            _connection = connection;
            _chat = connection.GetChatManager()
                                         .CreateChat(AuctionId(itemId), 
                                         null);
        }

        public void Bid(int amount)
        {
            SendMessage(string.Format(ApplicationConstants.BidFormat, amount));
        }

        public void Join()
        {
            SendMessage(ApplicationConstants.JoinCommandFormat);
        }

        private void SendMessage(string message)
        {
            _chat.SendMessage(message);
        }

        public void AddAuctionEventListener(IAuctionEventListener eventListener)
        {
            _chat.AddMessageListener(new AuctionMessageTranslator(_connection.User, eventListener).ProcessMessage);            
        }

        private static string AuctionId(string itemId)
        {
            return string.Format(ItemIdAsLogin, itemId);
        }
    }
}