using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AuctionSniper.Domain
{
    public class SnipersTableModel : ObservableCollection<SnipersTableModel.TableItem>
    {
        public SnipersTableModel()
        {
            //Add(new TableItem(SniperSnapshot.Joining("")));
        }

        public void SniperStatusChanged(SniperSnapshot newSnapshot)
        {            
            int row = GetRowForSniper(newSnapshot);
            SetItem(row, new TableItem(newSnapshot));
        }

        private int GetRowForSniper(SniperSnapshot newSnapshot)
        {
            var item = Items.Single(i => i.ItemId == newSnapshot.ItemId);
            return Items.IndexOf(item);
        }

        public static string TextFor(SniperState state)
        {
            return TableItem.TextFor(state);
        }

        public void AddSniper(SniperSnapshot joining)
        {
            Add(new TableItem(joining));
        }

        public class TableItem
        {
            private static readonly string[] StatusText =
            {
                "Joining", 
                "Bidding", 
                "Winning",
                "Lost",
                "Won"
            };

            public static string TextFor(SniperState state)
            {
                return StatusText[(int)state];
            }
    
            public TableItem(SniperSnapshot snapshot)
            {
                ItemId = snapshot.ItemId;
                LastPrice = snapshot.LastPrice;
                LastBid = snapshot.LastBid;
                State = StatusText[(int) snapshot.State];
            }
            
            [DisplayName("Item")]
            public string ItemId { get; private set; }
            
            [DisplayName("Last Price")]
            public int LastPrice { get; private set; }

            [DisplayName("Last Bid")]
            public int LastBid { get; private set; }           

            public string State { get; private set; }            
        }        
    }
}