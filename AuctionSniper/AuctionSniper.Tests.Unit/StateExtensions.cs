using System;
using System.Linq.Expressions;
using Moq;

namespace AuctionSniper.Tests.Unit
{
    public static class StateExtensions
    {
        public static void Allow<T>(this Mock<T> mock, Expression<Action<T>> allowedAction, Action setState) where T : class
        {
            mock.Setup(allowedAction)
                .Callback(setState);
        }

        public static void RestrictState<T>(this Mock<T> mock, Expression<Action<T>> expectedAction, Action checkState) where T : class
        {
            mock.Setup(expectedAction)
                .Callback(checkState);
        }
    }
}