﻿namespace AuctionSniper.Domain
{
    public interface ISniperListener
    {
        void SniperLost();
        void SniperBidding();
        void SniperWinning();
        void SniperWon();
    }
}