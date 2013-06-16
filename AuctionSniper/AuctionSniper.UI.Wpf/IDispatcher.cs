using System;

namespace AuctionSniper.UI.Wpf
{
    public interface IDispatcher
    {
        void Invoke(Action action);
    }
}