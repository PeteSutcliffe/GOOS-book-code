using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using White.Core.UIItems;
using White.Core.UIItems.ListViewItems;

namespace AuctionSniper.Tests.Acceptance
{
    public static class ListViewExtensions
    {
        public static ListViewCells GetCells(this ListViewRow row, ListViewHeader header)
        {

            AutomationElementCollection coll = row.AutomationElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "DataGridCell"));

            List<AutomationElement> list = coll.Cast<object>().Cast<AutomationElement>().ToList();

            return new ListViewCells(list, row.ActionListener, header);

        }
    }
}