namespace AuctionSniper.Domain
{
    public class Sniperstate
    {
        public string ItemId { get; private set; }
        public int LastPrice { get; private set; }
        public int LastBid { get; private set; }

        public Sniperstate(string itemId, int lastPrice, int lastBid)
        {
            ItemId = itemId;
            LastPrice = lastPrice;
            LastBid = lastBid;
        }

        protected bool Equals(Sniperstate other)
        {
            return string.Equals(ItemId, other.ItemId) && LastPrice == other.LastPrice && LastBid == other.LastBid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Sniperstate) obj);
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
    }
}