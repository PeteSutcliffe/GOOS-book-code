﻿namespace AuctionSniper.Domain
{
    public static class ApplicationConstants
    {        
        public const string CloseFormat = "SOLVersion: 1.1; Event: CLOSE;";
        public const string BidFormat = "SOLVersion: 1.1; Command: BID; Price {0}";
        public const string JoinCommandFormat = "SOLVersion: 1.1; Command: JOIN;";

        public const string StatusBidding = "Bidding";
        public const string StatusLost = "Lost";
        public const string StatusJoining = "Joining";
        public const string StatusWinning = "Winning";
        public const string StatusWon = "Won";
    }
}
