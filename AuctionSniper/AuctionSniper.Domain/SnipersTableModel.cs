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
            SniperStatus
        }

        public SnipersTableModel()
        {
            for (var i = 0; i < Enum.GetValues(typeof(Column)).Length; i++)
                Columns.Add();

            Columns[(int) Column.LastBid].DataType = typeof (int);
            Columns[(int) Column.LastPrice].DataType = typeof (int);

            Rows.Add(NewRow());
        }

        public void SetStatusText(string status)
        {
            Rows[0].BeginEdit();
            Rows[0][(int)Column.SniperStatus] = status;
            Rows[0].AcceptChanges();
        }

        public void SniperStatusChanged(Sniperstate sniperstate, string newStatusText)
        {
            Rows[0].BeginEdit();
            Rows[0][(int)Column.ItemIdentifier] = sniperstate.ItemId;
            Rows[0][(int)Column.LastPrice] = sniperstate.LastPrice;
            Rows[0][(int)Column.LastBid] = sniperstate.LastBid;
            Rows[0][(int)Column.SniperStatus] = newStatusText;
            Rows[0].AcceptChanges();
        }
    }
}