using AuctionSniper.Domain;

namespace AuctionSniper.XMPP
{
    public class XMPPAuctionHouse
    {
        private readonly Connection _connection;

        public XMPPAuctionHouse(string hostName, string userName)
        {
            _connection = new Connection(hostName, userName);
            _connection.Connect();
        }

        public XMPPAuction AuctionFor(Item item)
        {
            return new XMPPAuction(_connection, item.Identifier);
        }

        public void Disconnect()
        {
            _connection.Disconnect();
        }
    }
}