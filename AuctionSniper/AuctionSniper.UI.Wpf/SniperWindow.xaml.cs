using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AuctionSniper.Domain;
using AuctionSniper.UI.Wpf.Annotations;

namespace AuctionSniper.UI.Wpf
{
    public partial class SniperWindow
    {
        private SnipersTableModel _model;

        public SniperWindow()
        {
            InitializeComponent();

            _model = new SnipersTableModel();
            _model.SetStatusText(ApplicationConstants.StatusJoining);

            grid.ItemsSource = _model;
        }

        public void ShowStatus(string status)
        {
            //Dispatcher.BeginInvoke((Action)delegate { SniperStatus.Text = status; });
            _model.SetStatusText(status);
        }

        public class SnipersTableModel : IEnumerable<SniperRow>
        {
            private readonly List<SniperRow> _rows;

            public SnipersTableModel()
            {
                _rows = new List<SniperRow>();
                _rows.Add(new SniperRow());
            }

            public IEnumerator<SniperRow> GetEnumerator()
            {
                return _rows.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void SetStatusText(string text)
            {
                _rows[0].StatusText = text;
            }
        }
    }

    public class SniperRow : INotifyPropertyChanged
    {
        private string _statusText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
