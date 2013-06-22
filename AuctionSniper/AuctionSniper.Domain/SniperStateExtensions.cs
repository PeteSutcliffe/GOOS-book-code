namespace AuctionSniper.Domain
{
    public static class SniperStateExtensions
    {
        public static SniperState WhenAuctionClosed(this SniperState state)
        {
            switch (state)
            {
                case SniperState.Joining:
                case SniperState.Bidding:
                case SniperState.Losing:
                    return SniperState.Lost;                                    
                case SniperState.Winning:
                    return SniperState.Won;
                default:
                    throw new DefectException("Auction is already closed");
            }
        }
    }
}