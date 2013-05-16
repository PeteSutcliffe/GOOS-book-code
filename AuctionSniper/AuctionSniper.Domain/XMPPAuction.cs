using AuctionSniper.XMPP;

namespace AuctionSniper.Domain
{
    public class XMPPAuction : IAuction
    {
        private readonly Chat _chat;

        public XMPPAuction(Chat chat)
        {
            _chat = chat;
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
    }
}