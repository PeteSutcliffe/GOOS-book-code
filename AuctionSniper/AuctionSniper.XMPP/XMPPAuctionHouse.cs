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

        public XMPPAuction AuctionFor(string itemId)
        {
            return new XMPPAuction(_connection, itemId);
        }
    }
}