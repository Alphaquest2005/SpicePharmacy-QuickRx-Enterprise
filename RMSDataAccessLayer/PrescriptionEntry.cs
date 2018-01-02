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
    using System.ComponentModel;
    using TrackableEntities;
    using System;
    using System.Collections.Generic;
    using TrackableEntities.Client;
    
    public partial class PrescriptionEntry : TransactionEntryBase
    {
        
    	public string Dosage
    	{ 
    		get { return _Dosage; }
    		set
    		{
    			if (Equals(value, _Dosage)) return;
    			_Dosage = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _Dosage;
        
    	public Nullable<System.DateTime> ExpiryDate
    	{ 
    		get { return _ExpiryDate; }
    		set
    		{
    			if (Equals(value, _ExpiryDate)) return;
    			_ExpiryDate = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private Nullable<System.DateTime> _ExpiryDate;
        
    	public string Repeat
    	{ 
    		get { return _Repeat; }
    		set
    		{
    			if (Equals(value, _Repeat)) return;
    			_Repeat = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _Repeat;
        
    	public Nullable<int> RepeatCount
    	{ 
    		get { return _RepeatCount; }
    		set
    		{
    			if (Equals(value, _RepeatCount)) return;
    			_RepeatCount = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private Nullable<int> _RepeatCount;
    }
}
