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
                ApplicationConstants.StatusJoining, 
                ApplicationConstants.StatusBidding, 
                ApplicationConstants.StatusWinning,
                ApplicationConstants.StatusLost,
                ApplicationConstants.StatusWon
            };

        public SnipersTableModel()
        {
            for (var i = 0; i < Enum.GetValues(typeof(Column)).Length; i++)
                Columns.Add();

            Columns[(int) Column.LastBid].DataType = typeof (int);
            Columns[(int) Column.LastPrice].DataType = typeof (int);

            Rows.Add(NewRow());
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
    }
}