using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;

namespace AuctionSniper.Tests.Unit
{
    /// <summary>
    /// An attempt to emulate the state tracking outlined in the book.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MockWithState<T> : Mock<T> where T : class
    {
        private string _state;
        private readonly List<Tuple<Expression<Action<T>>, Times>> _restricted = new List<Tuple<Expression<Action<T>>, Times>>();

        public void Allow(Expression<Action<T>> allowedAction, string state)
        {
            Setup(allowedAction)
                .Callback(() => _state = state);
        }

        public void RestrictState(Expression<Action<T>> expectedAction, string allowedState)
        {
            RestrictState(expectedAction, allowedState, Times.AtLeastOnce());
        }

        public void RestrictState(Expression<Action<T>> expectedAction, string allowedState, Times times)
        {
            _restricted.Add(new Tuple<Expression<Action<T>>,Times>(expectedAction, times));
            Setup(expectedAction)
                .Callback(() => Assert.That(_state, Is.EqualTo(allowedState)));
        }

        public void VerifyRestrictedActions()
        {
            foreach (var tuple in _restricted)
            {
                Verify(tuple.Item1, tuple.Item2);
            }
        }
    }
}