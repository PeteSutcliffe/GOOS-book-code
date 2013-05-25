namespace AuctionSniper.Domain
{
    public static class SniperStateExtensions
    {
        public static SniperState WhenAuctionClosed(this SniperState state)
        {
            switch (state)
            {
                case SniperState.Joining:
                    return SniperState.Lost;
                case SniperState.Bidding:
                    return SniperState.Lost;
                case SniperState.Winning:
                    return SniperState.Won;
                default:
                    throw new DefectException("Auction is already closed");
            }
        }
    }
}