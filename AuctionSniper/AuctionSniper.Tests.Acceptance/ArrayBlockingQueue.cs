using System.Collections.Concurrent;

namespace AuctionSniper.Tests.Acceptance
{
    public class ArrayBlockingQueue<T> : BlockingCollection<T>
    {

        public ArrayBlockingQueue()
            : base(new ConcurrentQueue<T>())
        {
        }

        public ArrayBlockingQueue(int maxSize)
            : base(new ConcurrentQueue<T>(), maxSize)
        {
        }
    }
}