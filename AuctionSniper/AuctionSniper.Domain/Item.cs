namespace AuctionSniper.Domain
{
    public class Item
    {
        public string Identifier { get; private set; }
        public int StopPrice { get; private set; }

        public Item(string identifier, int stopPrice)
        {
            Identifier = identifier;
            StopPrice = stopPrice;
        }

        public bool AllowsBid(int bid)
        {
            return bid <= StopPrice;
        }

        protected bool Equals(Item other)
        {
            return string.Equals(Identifier, other.Identifier) && StopPrice == other.StopPrice;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^ StopPrice;
            }
        }
    }
}