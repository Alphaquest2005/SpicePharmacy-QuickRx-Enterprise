﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RMSDataAccessLayer
{
    public partial class TransactionBase: ISearchItem, IDataErrorInfo
    {
        public void UpdatePrices()
        {
            NotifyPropertyChanged("Amount");
            NotifyPropertyChanged("TotalSales");
            NotifyPropertyChanged("TotalTax");
            NotifyPropertyChanged("TotalDiscount");
        }

        PrescriptionEntry _currentTransactionEntry;
        public PrescriptionEntry CurrentTransactionEntry
        {
            get { return _currentTransactionEntry; }
            set
            {
                _currentTransactionEntry = value;
                if (_currentTransactionEntry != null)
                    _currentTransactionEntry.PropertyChanged += _currentTransactionEntry_PropertyChanged;
                NotifyPropertyChanged("CurrentTransactionEntry");
            }

        }

        void _currentTransactionEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Quantity" || e.PropertyName == "Price")
            {
                NotifyPropertyChanged("TotalSales");
                NotifyPropertyChanged("TotalTax");
                
            }
        }
       //public string TransactionNumber
       // {
       //     get
       //     {
       //         return TransactionId.ToString();
       //         //BarCodes.UPCA.cUPCA barcode = new BarCodes.UPCA.cUPCA();
       //         //string txnnumber = TransactionId.ToString().PadLeft(11, '0');
       //         //string finaltxn = txnnumber + barcode.GetCheckSum(txnnumber).ToString();
       //         //return finaltxn;
       //     }
       // }

       public Decimal TotalSales
       {
           get
           {
               if (TransactionEntries != null)
                        return TransactionEntries.Sum(x => x.Amount);
               return 0;
           }
       }
       public Decimal TotalTax
       {
           get
           {
               if (TransactionEntries != null)
                        return TransactionEntries.Where(t => t.Taxable).Sum(x => x.SalesTax);
               return 0;
           }

       }

       public Decimal TotalDiscount
       {
           get
           {
               if (TransactionEntries!= null)
                        return (decimal)TransactionEntries.Sum(x => x.Discount);
               return 0;
           }
       }



     
        public string SearchCriteria
        {
            get { return TransactionNumber + "|" + Time.ToShortDateString() + "|" + Time.ToShortTimeString() + "|" + Time.ToString("MMM"); }
           
        }

        public string DisplayName
        {
            get { return "TransactionList"; }
        }

        public string Key
        {
            get { return "TransactionList"; }
        }


        #region "Validation"
        Dictionary<string, string> m_validationErrors = new Dictionary<string, string>();
        public void AddError(string col, string msg)
        {
            if (!m_validationErrors.ContainsKey(col))
            {
                m_validationErrors.Add(col, msg);
            }
        }
        public void RemoveError(string col)
        {
            if (m_validationErrors.ContainsKey(col))
            {
                m_validationErrors.Remove(col);
            }
        }
        public string ValidationErrorMsg { get; set; }
        public virtual string Error
        {
            get 
            {
                if (m_validationErrors.Count > 0)
                {
                    return ValidationErrorMsg;
                }
                else
                {
                    return null;
                }
            }
        }

        public string this[string columnName]
        {
            get 
            {
                if (m_validationErrors.ContainsKey(columnName))
                {
                    return m_validationErrors[columnName];
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion
    }

}
