using System.Data;

using AuctionSniper.Domain;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        private readonly SnipersTableModel _model;

        public SniperWindow()
        {
            InitializeComponent();

            _model = new SnipersTableModel();
            _model.SetStatusText(ApplicationConstants.StatusJoining);

            grid.ItemsSource = _model.DefaultView;
        }

        public void ShowStatus(string status)
        {
            _model.SetStatusText(status);
        }

        public class SnipersTableModel : DataTable
        {
            public SnipersTableModel()
            {
                Columns.Add();
                Rows.Add(NewRow());
            }

            public void SetStatusText(string status)
            {
                Rows[0].BeginEdit();
                Rows[0][0] = status;
                Rows[0].AcceptChanges();
            }
        }        
    }
}
