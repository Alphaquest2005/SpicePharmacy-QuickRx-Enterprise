using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace QuickBooks
{
    class QuickBooksPOSViewModel: INotifyPropertyChanged
    {
        QBPOS qb = new QBPOS();
        public void DoInventoryItems()
        {
            qb.GetInventoryItemQuery();
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
