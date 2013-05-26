namespace AuctionSniper.Domain
{
    public static class ApplicationConstants
    {
        public const string CloseFormat = "SOLVersion: 1.1; Event: CLOSE;";
        public const string BidFormat = "SOLVersion: 1.1; Command: BID; Price {0}";
        public const string JoinCommandFormat = "SOLVersion: 1.1; Command: JOIN;";
    }
}
