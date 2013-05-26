using System;
using System.Data;

namespace AuctionSniper.Domain
{
    public class SnipersTableModel : DataTable
    {
        public enum Column
        {            
            ItemIdentifier,
            LastPrice,
            LastBid,
            SniperState
        }

        private static readonly string[] StatusText =
            {
                "Joining", 
                "Bidding", 
                "Winning",
                "Lost",
                "Won"
            };

        private static readonly string[] ColumnText =
            {
                "Item",
                "Last Price",
                "Last Bid",
                "State"
            };

        public SnipersTableModel()
        {
            for (var i = 0; i < Enum.GetValues(typeof(Column)).Length; i++)
                Columns.Add();

            Columns[(int) Column.LastBid].DataType = typeof (int);
            Columns[(int) Column.LastPrice].DataType = typeof (int);

            Rows.Add(NewRow());
            SniperStatusChanged(new SniperSnapshot("", 0, 0, SniperState.Joining));
        }

        public void SniperStatusChanged(SniperSnapshot newSnapshot)
        {
            Rows[0].BeginEdit();
            Rows[0][(int)Column.ItemIdentifier] = newSnapshot.ItemId;
            Rows[0][(int)Column.LastPrice] = newSnapshot.LastPrice;
            Rows[0][(int)Column.LastBid] = newSnapshot.LastBid;
            Rows[0][(int)Column.SniperState] = StatusText[(int)newSnapshot.State];
            Rows[0].AcceptChanges();
        }

        public static string TextFor(SniperState state)
        {
            return StatusText[(int) state];
        }

        public static string TextFor(Column column)
        {
            return ColumnText[(int)column];
        }
    }
}