//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMSDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransactionsView
    {
        public int TransactionId { get; set; }
        public System.DateTime Time { get; set; }
        public string ReferenceNumber { get; set; }
        public Nullable<decimal> TotalSales { get; set; }
        public Nullable<int> CustomerId { get; set; }
    }
}