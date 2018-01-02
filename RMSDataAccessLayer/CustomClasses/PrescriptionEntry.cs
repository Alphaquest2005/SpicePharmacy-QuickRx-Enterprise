using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Converters;
using System.Windows.Data;

namespace RMSDataAccessLayer
{
    public partial class PrescriptionEntry : ISearchItem
    {

        private string repeatMaster;
        public string RepeatMaster
        {
            get { return Repeat; }
            set
            {
                if (repeatMaster != value)
                {
                    repeatMaster = value;
                    int ival = 0;
                    Int32.TryParse(value,out ival);
                    if (ival != 0)
                    {
                        RepeatCount = ival;
                        Repeat = ival.ToString();
                    }
                    else
                    {
                        Repeat = value;
                    }
                    NotifyPropertyChanged("RepeatMaster");
                }
            }
        }

        

        #region ISearchItem Members

        public string SearchCriteria
        {
            get
            {
                return DisplayName + "|";
            }
            set
            {
               
            }
        }

        public string DisplayName
        {
            get { return ""; } //this.Item.DisplayName; 
            }

        public string Key
        {
            get { return TransactionEntryNumber; }
        }

        #endregion
    }
}
