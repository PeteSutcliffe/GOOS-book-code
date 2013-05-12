namespace AuctionSniper.XMPP
{
    public class Message
    {
        private readonly string _body;

        public Message()
        {
            
        }

        public Message(string body)
        {
            _body = body;
        }

        public string Body
        {
            get
            {
                return _body;
            }
        }
    }
}