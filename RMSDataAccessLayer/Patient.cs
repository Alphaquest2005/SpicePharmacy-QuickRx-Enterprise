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
    
    public partial class Patient : Person
    {
        public Patient()
        {
            this.Prescription = new ChangeTrackingCollection<Prescription>();
            CustomStartup();
            this.PropertyChanged += UpdatePropertyChanged;
        }
        partial void CustomStartup();
    
            private void UpdatePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.PropertyName) && (!Environment.StackTrace.Contains("Internal.Materialization")) && TrackingState == TrackingState.Unchanged)
                {
                    TrackingState = TrackingState.Modified;
                }
            }
        
    	public string Allergies
    	{ 
    		get { return _Allergies; }
    		set
    		{
    			if (Equals(value, _Allergies)) return;
    			_Allergies = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _Allergies;
        
    	public string Guardian
    	{ 
    		get { return _Guardian; }
    		set
    		{
    			if (Equals(value, _Guardian)) return;
    			_Guardian = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private string _Guardian;
        
    	public Nullable<double> Discount
    	{ 
    		get { return _Discount; }
    		set
    		{
    			if (Equals(value, _Discount)) return;
    			_Discount = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private Nullable<double> _Discount;
    
    	public ChangeTrackingCollection<Prescription> Prescription
    	{
    		get { return _Prescription; }
    		set
    		{
    			if (Equals(value, _Prescription)) return;
    			_Prescription = value;
    			NotifyPropertyChanged();
    		}
    	}
    	private ChangeTrackingCollection<Prescription> _Prescription;
    }
}
