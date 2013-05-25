using System;

namespace AuctionSniper.Domain
{
    public class SniperSnapshot
    {
        public string ItemId { get; private set; }
        public int LastPrice { get; private set; }
        public int LastBid { get; private set; }
        public SniperState State { get; private set; }

        public SniperSnapshot(string itemId, int lastPrice, int lastBid, SniperState sniperState)
        {
            State = sniperState;
            ItemId = itemId;
            LastPrice = lastPrice;
            LastBid = lastBid;
        }

        public static SniperSnapshot Joining(string itemId)
        {
            return new SniperSnapshot(itemId, 0, 0, SniperState.Joining);
        }

        public SniperSnapshot Winning(int newLastPrice)
        {
            return new SniperSnapshot(ItemId, newLastPrice, LastBid, SniperState.Winning);
        }

        public SniperSnapshot Bidding(int newLastPrice, int bid)
        {
            return new SniperSnapshot(ItemId, newLastPrice, bid, SniperState.Bidding);
        }

        public SniperSnapshot Closed()
        {
            return new SniperSnapshot(ItemId, LastPrice, LastBid, State.WhenAuctionClosed());
        }

        #region Equality
        protected bool Equals(SniperSnapshot other)
        {
            return string.Equals(ItemId, other.ItemId) && LastPrice == other.LastPrice && LastBid == other.LastBid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SniperSnapshot) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (ItemId != null ? ItemId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ LastPrice;
                hashCode = (hashCode*397) ^ LastBid;
                return hashCode;
            }
        }
        #endregion

    }

    public class DefectException : Exception
    {
        public DefectException(string message) : base(message)
        {
        }
    }
}