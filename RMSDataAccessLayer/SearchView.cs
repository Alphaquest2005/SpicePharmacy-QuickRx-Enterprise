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
    
    public partial class SearchView : EntityBase
    {
        
    	public System.DateTime Time
    	{ 
    		get { return _Time; }
    		set
    		{
    			if (Equals(value, _Time)) return;
    			_Time = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private System.DateTime _Time;
        
    	public int TransactionId
    	{ 
    		get { return _TransactionId; }
    		set
    		{
    			if (Equals(value, _TransactionId)) return;
    			_TransactionId = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private int _TransactionId;
        
    	public int ItemId
    	{ 
    		get { return _ItemId; }
    		set
    		{
    			if (Equals(value, _ItemId)) return;
    			_ItemId = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private int _ItemId;
        
    	public string ItemInfo
    	{ 
    		get { return _ItemInfo; }
    		set
    		{
    			if (Equals(value, _ItemInfo)) return;
    			_ItemInfo = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _ItemInfo;
        
    	public int PatientId
    	{ 
    		get { return _PatientId; }
    		set
    		{
    			if (Equals(value, _PatientId)) return;
    			_PatientId = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private int _PatientId;
        
    	public string PatientInfo
    	{ 
    		get { return _PatientInfo; }
    		set
    		{
    			if (Equals(value, _PatientInfo)) return;
    			_PatientInfo = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _PatientInfo;
        
    	public int DoctorId
    	{ 
    		get { return _DoctorId; }
    		set
    		{
    			if (Equals(value, _DoctorId)) return;
    			_DoctorId = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private int _DoctorId;
        
    	public string DoctorInfo
    	{ 
    		get { return _DoctorInfo; }
    		set
    		{
    			if (Equals(value, _DoctorInfo)) return;
    			_DoctorInfo = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _DoctorInfo;
        
    	public string SearchInfo
    	{ 
    		get { return _SearchInfo; }
    		set
    		{
    			if (Equals(value, _SearchInfo)) return;
    			_SearchInfo = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _SearchInfo;
    
    	public Prescription Prescription
    	{
    		get { return _Prescription; }
    		set
    		{
    			if (Equals(value, _Prescription)) return;
    			_Prescription = value;
    			PrescriptionChangeTracker = _Prescription == null ? null
    				: new ChangeTrackingCollection<Prescription> { _Prescription };
    			NotifyPropertyChanged();
    		}
    	}
    	private Prescription _Prescription;
    	private ChangeTrackingCollection<Prescription> PrescriptionChangeTracker { get; set; }
    }
}
