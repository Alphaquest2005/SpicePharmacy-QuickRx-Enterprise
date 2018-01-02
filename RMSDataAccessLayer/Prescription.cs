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
    
    public partial class Prescription : TransactionBase
    {
        
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
    
    	public Doctor Doctor
    	{
    		get { return _Doctor; }
    		set
    		{
    			if (Equals(value, _Doctor)) return;
    			_Doctor = value;
    			DoctorChangeTracker = _Doctor == null ? null
    				: new ChangeTrackingCollection<Doctor> { _Doctor };
    			NotifyPropertyChanged();
    		}
    	}
    	private Doctor _Doctor;
    	private ChangeTrackingCollection<Doctor> DoctorChangeTracker { get; set; }
    
    	public Patient Patient
    	{
    		get { return _Patient; }
    		set
    		{
    			if (Equals(value, _Patient)) return;
    			_Patient = value;
    			PatientChangeTracker = _Patient == null ? null
    				: new ChangeTrackingCollection<Patient> { _Patient };
    			NotifyPropertyChanged();
    		}
    	}
    	private Patient _Patient;
    	private ChangeTrackingCollection<Patient> PatientChangeTracker { get; set; }
    
    	public SearchView SearchViews
    	{
    		get { return _SearchViews; }
    		set
    		{
    			if (Equals(value, _SearchViews)) return;
    			_SearchViews = value;
    			SearchViewsChangeTracker = _SearchViews == null ? null
    				: new ChangeTrackingCollection<SearchView> { _SearchViews };
    			NotifyPropertyChanged();
    		}
    	}
    	private SearchView _SearchViews;
    	private ChangeTrackingCollection<SearchView> SearchViewsChangeTracker { get; set; }
    }
}
