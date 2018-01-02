using System.Linq;

using RMSDataAccessLayer;

using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Design;
using System.Drawing.Printing;
using System.Windows.Data;
using System.Printing;
using System.Threading.Tasks;
using SUT.PrintEngine.Utils;
using System.Windows.Media;
using log4netWrapper;
using QuickBooks;
using SalesRegion.Messages;
using SimpleMvvmToolkit;
using TrackableEntities;
using TrackableEntities.Common;
using TrackableEntities.EF6;

namespace SalesRegion
{
    public class SalesVM : ViewModelBase<SalesVM>
    {


        private static readonly SalesVM _instance;

        static SalesVM()
        {
            _instance = new SalesVM();
        }

        public static SalesVM Instance
        {
            get { return _instance; }
        }


        private static Cashier _cashier;

        public Cashier CashierEx
        {
            get { return _cashier; }
            set
            {
                if (_cashier != value)
                {
                    _cashier = value;
                    NotifyPropertyChanged(x => x.CashierEx);
                }
            }
        }

        public SalesVM()
        {

        }


        public void CloseTransaction()
        {
            try
            {
                Logger.Log(LoggingLevel.Info, "Close Transaction");
                if (batch == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Batch is null");
                    MessageBox.Show("Batch is null");
                    return;
                }
                if (TransactionData != null)
                {
                    TransactionData.CloseBatchId = Batch.BatchId;
                    TransactionData.OpenClose = false;

                    SaveTransaction();
                    TransactionData = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void CreateNewPrescription()
        {
            try
            {

                Logger.Log(LoggingLevel.Info, "Create New Prescription");
                if (doctor == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Doctor is Missing");
                    this.Status = "Doctor is Missing";
                    return;
                }

                if (patient == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Patient is Missing");
                    this.Status = "Patient is Missing";
                    return;
                }

                if (Store == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Store is Missing");
                    this.Status = "Store is Missing";
                    return;
                }

                if (Batch == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Batch is Missing");
                    this.Status = "Batch is Missing";
                    return;
                }

                if (CashierEx == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Cashier is Missing");
                    this.Status = "CashierEx is Missing";
                    return;
                }

                if (Station == null)
                {
                    Logger.Log(LoggingLevel.Warning, "Station is Missing");
                    this.Status = "Station is Missing";
                    return;
                }
                Prescription txn = new Prescription()
                {
                    BatchId = Batch.BatchId,
                    StationId = Station.StationId,
                    Time = DateTime.Now,
                    CashierId = CashierEx.Id,
                    PharmacistId = (CashierEx.Role == "Pharmacist" ? CashierEx.Id : null as int?),
                    StoreCode = Store.StoreCode,
                    OpenClose = true,
                    DoctorId = doctor.Id,
                    PatientId = patient.Id,
                    Patient = patient,
                    Doctor = doctor,
                    Cashier = CashierEx,
                    Pharmacist = CashierEx.Role == "Pharmacist" ? CashierEx : null,
                    TrackingState = TrackingState.Added
                };
                Logger.Log(LoggingLevel.Info, "Prescription Created");
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        //+ ToDo: Replace this with your own data fields

        private Doctor doctor = null;

        public Doctor Doctor
        {
            get { return doctor; }
            set
            {
                if (doctor != value)
                {
                    doctor = value;
                    NotifyPropertyChanged(x => x.Doctor);
                }
            }
        }

        private Patient patient = null;

        public Patient Patient
        {
            get { return patient; }
            set
            {
                if (patient != value)
                {
                    patient = value;
                    NotifyPropertyChanged(x => x.Patient);
                }
            }
        }

        private Cashier transactionCashier = null;
        public Cashier TransactionCashier
        {
            get { return transactionCashier; }
            set
            {
                if (transactionCashier != value)
                {
                    transactionCashier = value;
                    NotifyPropertyChanged(x => x.TransactionCashier);
                }
            }
        }

        private Cashier _transactionPharmacist = null;
        public Cashier TransactionPharmacist
        {
            get { return _transactionPharmacist; }
            set
            {
                if (_transactionPharmacist != value)
                {
                    _transactionPharmacist = value;
                    NotifyPropertyChanged(x => x.TransactionPharmacist);
                }
            }
        }

        private string status = null;

        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    NotifyPropertyChanged(x => x.Status);
                }
            }
        }


        public TransactionBase transactionData;

        public TransactionBase TransactionData
        {
            get { return transactionData; }
            set
            {
                if (!object.Equals(transactionData, value))
                {
                    Set_TransactionData(value);

                }
            }
        }

        private void Set_TransactionData(TransactionBase value)
        {
            transactionData = value;
            SetTransactionNumber(transactionData);
           SendMessage(MessageToken.TransactionDataChanged,
                new NotificationEventArgs<TransactionBase>(MessageToken.TransactionDataChanged, transactionData));


            NotifyPropertyChanged(x => x.TransactionData);
        }


        private ObservableCollection<object> _csv;

        public ObservableCollection<object> SearchList
        {
            get { return _csv; }

        }

        private ObservableCollection<Cashier> _pharmacists = null;

        public ObservableCollection<Cashier> Pharmacists
        {
            get
            {
                if (_pharmacists == null)
                {
                    using (var ctx = new RMSModel())
                    {
                        _pharmacists =
                            new ObservableCollection<Cashier>(
                                ctx.Persons.OfType<Cashier>().Where(x => x.Role == "Pharmacist"));
                    }
                }
                return _pharmacists;
            }
        }


        private Cashier _currentPharmacist = null;

        public Cashier CurrentPharmacist
        {
            get
            {
                return _currentPharmacist;
            }
            set
            {
                if (_currentPharmacist != value)
                {
                    _currentPharmacist = value;
                    NotifyPropertyChanged(x => CurrentPharmacist);
                }
            }
        }


        public void UpdateSearchList(string filterText)
        {
            try
            {
                Logger.Log(LoggingLevel.Info,
                    string.Format("Update SearchList -filter Text [{0}] - StartTime:{1}", filterText, DateTime.Now));
                CompositeCollection cc = CreateSearchList(filterText);


                _csv = new ObservableCollection<Object>();
                foreach (var item in cc)
                {
                    _csv.Add(item);
                }
                NotifyPropertyChanged(x => x.SearchList);
                Logger.Log(LoggingLevel.Info,
                    string.Format("Finish Update SearchList - filter Text [{0}] - EndTime:{1}", filterText, DateTime.Now));
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void GetSearchResults(string filterText)
        {
            UpdateSearchList(filterText);
        }



        private CompositeCollection CreateSearchList(string filterText)
        {
            try
            {
                //todo: make parallel
                Logger.Log(LoggingLevel.Info,
                    string.Format("Start Create SearchList -filter Text [{0}] - StartTime:{1}", filterText, DateTime.Now));
                CompositeCollection cc = new CompositeCollection();


                foreach (var itm in AddSearchItems())
                {
                    cc.Add(itm);
                }


                GetPatients(cc, filterText);
                GetDoctors(cc, filterText);

                AddInventory(cc, filterText);

                double t = 0;
                if (double.TryParse(filterText, out t))
                {
                    AddTransaction(cc, filterText);
                }

                Logger.Log(LoggingLevel.Info,
                    string.Format("Finish Create SearchList -filter Text [{0}] - StartTime:{1}", filterText,
                        DateTime.Now));
                return cc;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private CompositeCollection AddSearchItems()
        {
            try
            {
                CompositeCollection cc = new CompositeCollection();
                //SearchItem b = new SearchItem();
                //b.SearchObject = new RMSDataAccessLayer.Transactionlist();
                //b.SearchCriteria = "Transaction History";
                //b.DisplayName = "Transaction History";
                //cc.Add(b);

                SearchItem p = new SearchItem();
                p.SearchObject = null;
                p.SearchCriteria = "Add Patient";
                p.DisplayName = "Add Patient";
                cc.Add(p);

                SearchItem d = new SearchItem();
                d.SearchObject = null;
                d.SearchCriteria = "Add Doctor";
                d.DisplayName = "Add Doctor";
                cc.Add(d);


                return cc;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }





        private void AddTransaction(CompositeCollection cc, string filterText)
        {
            if (cc == null) return;
            try
            {
                using (var ctx = new RMSModel())
                {
                    // right now any prescriptions
                    foreach (
                        var trns in
                            ctx.TransactionBase.OfType<Prescription>()
                                .Where(x => x.TransactionId.ToString().Contains(filterText))
                                .OrderBy(t => t.Time)
                                .Take(100))
                    {
                        cc.Add(trns);
                    }
                }
                using (var ctx = new RMSModel())
                {
                    foreach (
                        var trns in
                            ctx.TransactionBase.OfType<QuickPrescription>()
                                .Where(x => x.TransactionId.ToString().Contains(filterText))
                                .OrderBy(t => t.Time)
                                .Take(100))
                    {
                        cc.Add(trns);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }






        private void GetDoctors(CompositeCollection cc, string filterText)
        {
            try
            {
                using (var ctx = new RMSModel())
                {
                    foreach (
                        var cus in
                            ctx.Persons.OfType<Doctor>()
                                .Where(
                                    x =>
                                        ("Dr. " + " " + x.FirstName.Trim().Replace(".", "").Replace(" ", "").Replace("Dr", "Dr. ") + " " +
                                         x.LastName +
                                         " " + x.Code).Contains(filterText))
                                .Take(listCount))
                    {
                        cc.Add(cus);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void GetPatients(CompositeCollection cc, string filterText)
        {
            try
            {
                using (var ctx = new RMSModel())
                {
                    foreach (
                        var cus in
                            ctx.Persons.OfType<Patient>()
                                .Where(x => (x.FirstName + " " + x.LastName).Contains(filterText))
                                .Take(listCount)) //
                    {
                        cc.Add(cus);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private bool _showInactiveItems = false;
        public bool ShowInactiveItems
        {
            get
            {
                return _showInactiveItems;
            }
            set
            {
                _showInactiveItems = value;
                NotifyPropertyChanged(x => x.ShowInactiveItems);
            }

        }
        private int listCount = 25;


        private void AddInventory(CompositeCollection cc, string filterText)
        {
            try
            {
                //todo: make parallel
                using (var ctx = new RMSModel())
                {

                    var itms = ctx.Item.OfType<Medicine>().Where(x => ((x.Description + "|" + x.ItemName ).Contains(filterText) )//|| (x.ItemNumber.ToString().Contains(filterText))
                                                                       && x.QBItemListID != null
                        // && x.Quantity > 0
                                                                       && (x.Inactive == null ||
                                                                          (x.Inactive != null && x.Inactive == _showInactiveItems)))
                                                                         
                         .Take(listCount)
                         .AsEnumerable()
                         .OrderBy(x => x.DisplayName).ToList();

                    foreach (var itm in itms)
                    {
                        cc.Add(itm);
                    }
                }

                using (var ctx = new RMSModel())
                {
                    foreach (
                        var itm in
                            ctx.Item.OfType<StockItem>()
                                .Where(x => (x.ItemName ?? x.Description).Contains(filterText))
                                .Take(listCount))
                    {
                        cc.Add(itm);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }


        }




        public void ProcessSearchListItem(object SearchItem)
        {
            try
            {
                if (SearchItem != null)
                {
                    if (typeof(RMSDataAccessLayer.SearchItem) == SearchItem.GetType())
                    {
                        DoSearchItem(SearchItem as RMSDataAccessLayer.SearchItem);
                    }

                    if (typeof(RMSDataAccessLayer.Doctor) == SearchItem.GetType())
                    {
                        AddDoctorToTransaction(SearchItem as Doctor);
                    }

                    if (typeof(RMSDataAccessLayer.Patient) == SearchItem.GetType())
                    {
                        AddPatientToTransaction(SearchItem as Patient);
                    }

                    if (SearchItem is Item)
                    {

                        var itm = (Item)SearchItem;
                        if (CheckDuplicateItem(itm)) return;
                        if (itm.Quantity < 0)
                        {
                            var res = MessageBox.Show("Item may not be in stock! Do you want to continue?", "Negative Stock",
                                MessageBoxButton.YesNo);
                            if (res == MessageBoxResult.No) return;
                        }
                        using (var ctx = new RMSModel())
                        {
                            itm.DosageList =
                                ctx.ItemDosages.Where(x => x.ItemId == itm.ItemId)
                                    .OrderByDescending(x => x.Count)
                                    .Take(5)
                                    .Select(x => x.Dosage)
                                    .ToList();
                            itm.TrackingState = TrackingState.Unchanged;
                        }

                        if (TransactionData != null)
                        {
                            InsertItemTransactionEntry(itm);
                        }
                        else
                        {
                            NewItemTransaction(itm);
                        }

                    }
                    if (SearchItem is TransactionBase)
                    {
                        GoToTransaction((TransactionBase)SearchItem);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private bool CheckDuplicateItem(Item itm)
        {
            if (TransactionData != null &&
                TransactionData.TransactionEntries.FirstOrDefault(x => x.ItemId == itm.ItemId) != null)
            {
                MessageBox.Show("Can't add same item twice!");
                return true;
            }
            return false;
        }

        private void DoSearchItem(SearchItem searchItem)
        {
            throw new NotImplementedException();
        }


        private void AddPatientToTransaction(Patient patient)
        {
            if (patient == null) return;
            Patient = patient;
            if (TransactionData is Prescription == false)
            {
                var t = NewPrescription();
                CopyTransactionDetails(t, TransactionData);
                DeleteTransactionData();
                TransactionData = t;
            }
            var prescription = (Prescription)TransactionData;
            if (prescription != null)
            {
                prescription.PatientId = patient.Id;
                prescription.Patient = patient;
            }

        }



        private void AddDoctorToTransaction(Doctor doctor)
        {
            if (doctor == null) return;
            Doctor = doctor;
            if (TransactionData is Prescription == false)
            {
                var t = NewPrescription();
                CopyTransactionDetails(t, TransactionData);
                DeleteTransactionData();
                TransactionData = t;
            }

            var prescription = TransactionData as Prescription;
            if (prescription != null)
            {
                prescription.DoctorId = doctor.Id;
                prescription.Doctor = doctor;
            }

        }

        private void DeleteTransactionData()
        {
            if (TransactionData != null && TransactionData.TrackingState != TrackingState.Added)
            {
                using (var ctx = new RMSModel())
                {
                    var t = ctx.TransactionBase.FirstOrDefault(x => x.TransactionId == TransactionData.TransactionId);
                    if (TransactionData != null)
                    {
                        t.TrackingState = TrackingState.Deleted;
                        ctx.ApplyChanges(t);
                        ctx.SaveChanges();
                    }
                    TransactionData.TrackingState = TrackingState.Deleted;
                    // TransactionData.AcceptChanges();
                }
            }
            TransactionData = null;
        }


        private void GoToTransaction(TransactionBase trn)
        {
            GoToTransaction(trn.TransactionId);
        }


        public void GoToTransaction(int TransactionId)
        {
            try
            {
                using (var ctx = new RMSModel())
                {
                    TransactionBase ntrn;
                    ntrn = (from t in ctx.TransactionBase
                        .Include(x => x.TransactionEntries)
                        .Include(x => x.Cashier)
                        .Include("TransactionEntries.Item")
                        //.Include("TransactionEntries.Item.ItemDosages")
                            where t.TransactionId == TransactionId
                            orderby t.Time descending
                            select t).FirstOrDefault();
                    if (ntrn != null)
                    {
                        
                        IncludePrecriptionProperties(ctx, ntrn);
                        Item = null;
                        NotifyPropertyChanged(x => x.Item.DosageList);
                        TransactionData = ntrn;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void GoToPreviousTransaction()
        {
            try
            {
                using (var ctx = new RMSModel())
                {
                    TransactionBase ptrn;

                    if (TransactionData == null)
                    {
                        ptrn = GetDBTransaction(ctx);
                    }
                    else
                    {
                        if (TransactionData.TransactionId != 0)
                        {
                            ptrn = ctx.TransactionBase.OrderByDescending(t => t.Time)
                                .Include(x => x.TransactionEntries)
                                .Include(x => x.Cashier)
                                .Include("TransactionEntries.Item")
                              //  .Include("TransactionEntries.Item.ItemDosages")

                                .FirstOrDefault(t => t.TransactionId < TransactionData.TransactionId);
                        }
                        else
                        {
                            ptrn = (from t in ctx.TransactionBase.Include(x => x.TransactionEntries)
                                .Include(x => x.Cashier)
                                .Include("TransactionEntries.Item")
                              //  .Include("TransactionEntries.Item.ItemDosages")

                                    orderby t.Time descending
                                    select t).FirstOrDefault();
                        }
                    }
                    if (ptrn != null)
                    {
                        IncludePrecriptionProperties(ctx, ptrn);
                        Item = null;
                        NotifyPropertyChanged(x => x.Item.DosageList);

                      //  IncludeInventoryProperties(ctx, ptrn);
                        TransactionData = ptrn;
                        this.Item = null;
                    }
                    else
                    {
                        MessageBox.Show("No previous transaction");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void IncludeInventoryProperties(RMSModel ctx, TransactionBase ptrn)
        {
            foreach (var itm in ptrn.TransactionEntries)
            {
                itm.Item.DosageList = ctx.ItemDosages.OrderByDescending(x => x.Count).Take(5).Select(x => x.Dosage).ToList();
            }
        }

        private TransactionBase GetDBTransaction(RMSModel ctx)
        {
            try
            {
                TransactionBase ptrn;
                ptrn = (from t in ctx.TransactionBase
                    .Include(x => x.TransactionEntries)
                    .Include(x => x.Cashier)
                    .Include("TransactionEntries.Item")
                    //.Include("TransactionEntries.Item.ItemDosages")
                        orderby t.Time descending
                        select t).FirstOrDefault();

                return ptrn;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void IncludePrecriptionProperties(TransactionBase ptrn)
        {
            try
            {
                if (ptrn == null) return;
                using (var ctx = new RMSModel())
                {
                    IncludePrecriptionProperties(ctx,ptrn);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void IncludePrecriptionProperties(RMSModel ctx, TransactionBase ptrn)
        {
            try
            {
                if (ptrn == null) return;
                if (ptrn is Prescription)
                {
                    var pc = (ptrn as Prescription);
                    pc.Doctor = ctx.Persons.OfType<Doctor>().FirstOrDefault(x => x.Id == pc.DoctorId);
                    pc.Patient = ctx.Persons.OfType<Patient>().FirstOrDefault(x => x.Id == pc.PatientId);

                }
                this.TransactionCashier = ctx.Persons.OfType<Cashier>().FirstOrDefault(x => x.Id == ptrn.CashierId);
                this.TransactionPharmacist = ctx.Persons.OfType<Cashier>().FirstOrDefault(x => x.Id == ptrn.PharmacistId);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private void InsertItemTransactionEntry(RMSDataAccessLayer.Item itm)
        {
            try
            {
                var medicine = itm as Medicine;
                if (TransactionData.CurrentTransactionEntry == null)
                {
                   
                        PrescriptionEntry p = new PrescriptionEntry()
                        {
                            StoreID = Store.StoreId,
                            TransactionId = TransactionData.TransactionId,
                            ItemId = itm.ItemId,

                            Price = itm.Price,
                            Dosage = medicine == null?"":medicine.SuggestedDosage,
                            Taxable = itm.SalesTax != 0,
                            SalesTaxPercent = itm.SalesTax.GetValueOrDefault(),
                            TransactionTime = DateTime.Now,
                            EntryNumber =
                                TransactionData.TransactionEntries == null
                                    ? 0
                                    : (short?)TransactionData.TransactionEntries.Count,
                            // Transaction = TransactionData,
                            Item = itm,
                            TrackingState = TrackingState.Added
                        };
                        TransactionData.TransactionEntries.Add(p);
                        this.TransactionData.CurrentTransactionEntry = p;
                    
                }
                else
                {
                    var item = this.TransactionData.CurrentTransactionEntry;
                    if (item != null)
                    {
                        item.ItemId = itm.ItemId;
                        item.Price = itm.Price;
                       
                        if (medicine != null) item.Dosage = medicine.SuggestedDosage;
                        item.Item = itm;
                        this.TransactionData.UpdatePrices();
                    }
                    
                    
                    this.Item = itm;
                }





                NotifyPropertyChanged(x => x.TransactionData);
                //NotifyPropertyChanged(x => x.CurrentTransactionEntry);
                NotifyPropertyChanged(x => x.Item);
                return;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private bool AutoCreateOldTransactions()
        {
            try
            {
                if (TransactionData == null) return false;
                if (TransactionData.Time.Date != DateTime.Now.Date)
                {
                    var res =
                        MessageBox.Show(
                            "Modifying old transactions is not allowed! Do you want to create a New Transaction?",
                            "Can't Modify Old Transaction", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        TransactionData = CopyCurrentTransaction();
                        return true;
                    }
                    if (res == MessageBoxResult.No) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void DeleteTransactionEntry<T>(TransactionEntryBase dtrn) where T : TransactionEntryBase
        {
            try
            {
                if (dtrn == null) return;
                if (AutoCreateOldTransactions() == false) return;

                using (var ctx = new RMSModel())
                {
                    var d = ctx.TransactionEntryBase.FirstOrDefault(x => x.TransactionEntryId == dtrn.TransactionEntryId);
                    if (d != null)
                    {
                        TransactionData.TransactionEntries.Remove(d);
                        ctx.TransactionEntryBase.Remove(d);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        TransactionData.TransactionEntries.Remove(dtrn);
                    }

                    NotifyPropertyChanged(x => TransactionData.TransactionEntries);
                    NotifyPropertyChanged(x => TransactionData);
                    TransactionData.UpdatePrices();

                }
                GoToTransaction(TransactionData.TransactionId);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void DeleteCurrentTransaction()
        {
            try
            {


                Logger.Log(LoggingLevel.Info,
                    string.Format("Start DeleteCurrentTransaction: StartTime:{0}", DateTime.Now));
                if (
                    MessageBox.Show("Are you sure you want to delete?", "Delete Current Transaction",
                        MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    if (TransactionData != null && TransactionData.Time.Date != DateTime.Now.Date)
                    {
                        MessageBox.Show("Modifying old transactions is not allowed!",
                            "Can't Modify Old Transaction");
                        return;
                    }
                    using (var ctx = new RMSModel())
                    {

                        var t = ctx.TransactionBase.FirstOrDefault(x => x.TransactionId == TransactionData.TransactionId);
                        if (t != null)
                        {
                            //ctx.TransactionBase.Remove(t);
                            t.TrackingState = TrackingState.Deleted;
                            ctx.ApplyChanges(t);
                            ctx.SaveChanges();
                            t.AcceptChanges();
                        }
                        TransactionData = null;
                        GoToPreviousTransaction();
                    }
                }
                Logger.Log(LoggingLevel.Info,
                    string.Format("Finish DeleteCurrentTransaction: EndTime:{0}", DateTime.Now));
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public TransactionBase CopyCurrentTransaction(bool copydetails = true)
        {
            try
            {
                dynamic newt = null;
                if (TransactionData is Prescription)
                {
                    var p = NewPrescription();
                    p.Doctor = (TransactionData as Prescription).Doctor;
                    p.Patient = (TransactionData as Prescription).Patient;
                    newt = p;
                }
                if (TransactionData is QuickPrescription)
                    newt = NewQuickPrescription();


                if (copydetails)
                {
                    CopyTransactionDetails(newt, TransactionData);
                }
                newt.UpdatePrices();
                return newt;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + " | " + ex.StackTrace);
                throw ex;
            }
        }

        private void CopyTransactionDetails(dynamic newt, TransactionBase t)
        {
            if (newt == null || t == null) return;
            foreach (var itm in t.TransactionEntries.OfType<PrescriptionEntry>())
            {
                var te = new PrescriptionEntry()
                {
                    Dosage = itm.Dosage,
                    ItemId = itm.ItemId,
                    Item = itm.Item,
                    Repeat = itm.Repeat,
                    RepeatCount = itm.RepeatCount,
                    Quantity = itm.Quantity,
                    SalesTaxPercent = itm.SalesTaxPercent,
                    Price = itm.Price,
                    ExpiryDate = itm.ExpiryDate,
                    Comment = itm.Comment,
                    TrackingState = TrackingState.Added
                };
                te.Item.TransactionEntryBase = null;
                newt.TransactionEntries.Add(te);
            }
        }



        public void AutoRepeat()
        {
            try
            {
                TransactionBase newt = CopyCurrentTransaction();
                foreach (PrescriptionEntry item in newt.TransactionEntries.ToList())
                {
                    if (item.RepeatCount == 0)
                    {
                        //newt.TransactionEntries.Remove(item);
                        // rms.Detach(item);
                    }
                    else
                    {
                        item.RepeatCount -= 1;
                        item.Repeat = item.RepeatCount.ToString();

                    }
                }



                // rms.TransactionBase.AddObject(newt);
                TransactionData = newt;
                SaveTransaction();
                SalesVM.Instance.GoToTransaction(newt.TransactionId);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }

        }


        private void NewItemTransaction(Item SearchItem)
        {
            try
            {
                if (CheckDuplicateItem(SearchItem)) return;
                if (TransactionData == null)
                {
                    TransactionData = NewQuickPrescription();
                }
                InsertItemTransactionEntry(SearchItem as Item);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private QuickPrescription NewQuickPrescription()
        {
            try
            {
                return new QuickPrescription()
                {
                    BatchId = Batch.BatchId,
                    StationId = Station.StationId,
                    Time = DateTime.Now,
                    CashierId = CashierEx.Id,
                    PharmacistId = (CashierEx.Role == "Pharmacist" ? CashierEx.Id : null as int?),
                    StoreCode = Store.StoreCode,
                    OpenClose = true ,
                    TrackingState = TrackingState.Added
                };
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void Print(ref Grid fwe)
        {
            FrameworkElement f = fwe;
            Print(ref f);
        }


        public void Print(ref FrameworkElement fwe)
        {
            try
            {
                if (TransactionData == null) return;
              //  LocalPrintServer printserver = new LocalPrintServer();
                PrintServer printserver = new PrintServer(Station.PrintServer);

                //RotateTransform myRotateTransform = new RotateTransform();
                //myRotateTransform.Angle = 270;
                //fwe.RenderTransform = myRotateTransform;
                //fwe.UpdateLayout();
                Size visualSize;
               


                visualSize = new Size(fwe.ActualWidth, fwe.ActualHeight< 270 ? 290:fwe.ActualHeight + 20); // paper size // 2 * 96//, fwe.ActualHeight + 5

                DrawingVisual visual = PrintControlFactory.CreateDrawingVisual(fwe,fwe.ActualWidth ,fwe.ActualHeight + 20);//fwe.ActualWidth// , 


                SUT.PrintEngine.Paginators.VisualPaginator page = new SUT.PrintEngine.Paginators.VisualPaginator(
                    visual, visualSize, new Thickness(0, 0, 0, 0), new Thickness(0, 0, 0, 0));
                page.Initialize(false);

                

                PrintDialog pd = new PrintDialog();
                //pd.ShowDialog();
                pd.PrintQueue = printserver.GetPrintQueue(Station.ReceiptPrinterName);
                pd.PrintTicket.PageOrientation = PageOrientation.Portrait;
               // pd.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName., pd.PrintTicket.PageMediaSize.Width.GetValueOrDefault(), fwe.ActualHeight / 96);
                pd.PrintDocument(page, "");

            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        public void PostQBSale()
        {

            try
            {

                if (TransactionData == null || string.IsNullOrEmpty(TransactionData.TransactionNumber))
                {
                    MessageBox.Show("Invalid Transaction Please Try again");
                    return;
                }
                
                    TransactionData.Status = "ToBePosted";
                    SaveTransaction();
                    if (ServerMode != true)
                    {
                        Post(TransactionData);
                    }

            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

       

        private void Post(TransactionBase trn)
        {
            try
            {
                if (trn == null) return;
                IncludePrecriptionProperties(trn);

                SalesReceipt s = new SalesReceipt();
                s.TxnDate = TransactionData.Time;
                s.TxnState = "1";
                s.Workstation = "02";
                s.StoreNumber = "1";
                s.SalesReceiptNumber = "123";
                s.Discount = "0";


                if (trn is Prescription)
                {
                    Prescription p = trn as Prescription;
                    string doctor = "";
                    string patient = "";
                    if (p.Doctor != null)
                    {
                        doctor = p.Doctor.DisplayName;
                    }
                    if (p.Patient != null)
                    {
                        patient = p.Patient.ContactInfo;
                        s.Discount = p.Patient.Discount == null ? "" : p.Patient.Discount.ToString();
                      
                    }
                    s.Comments = String.Format("{0} \n RX#:{1} \n Doctor:{2}", patient,
                    trn.TransactionNumber, doctor);

                }
                else
                {
                   s.Comments = "RX#:" + trn.TransactionNumber;
                }

                if (trn != null)
                {
                    s.TrackingNumber = trn.TransactionNumber;
                }
                s.Associate = "Dispensary";
                s.SalesReceiptType = "0";



                foreach (var item in trn.TransactionEntries)
                {
                    if (item.Item.QBItemListID != null)
                    {

                        s.SalesReceiptDetails.Add(new SalesReceiptDetail
                        {
                            ItemListID = item.Item.QBItemListID,
                           // ItemKey = item.Item.ItemName,
                            QtySold = item.Quantity,
                            ///TaxCode = item.Taxable == true?"Tax":"Non"
                            
                        }); //340 
                    }
                    else
                    {
                        ////MessageBox.Show("Please Link Quickbooks item to dispensary");
                        //trn.Status = "Please Link Quickbooks item to dispensary";
                        //rms.SaveChanges();
                        return;
                    }
                }


                // QBPOS qb = new QBPOS();
                
                SalesReceiptRet result = QBPOS.Instance.AddSalesReceipt(s);
                if (result != null)
                {
                    using (var ctx = new RMSModel())
                    {
                        trn.ReferenceNumber = "QB#" + result.SalesReceiptNumber;
                        trn.Status = "Posted";
                        ctx.TransactionBase.AddOrUpdate(trn);
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    // problem
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        

        public async Task DownloadAllQBItems()
        {
            try
            {
                await Task.Run(() =>
                {
                    var t = QBPOS.Instance.GetInventoryItemQuery();
                    Status = "QBResults Returned";
                    ProcessQBItems(t);
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private async Task ProcessQBItems(IEnumerable<ItemInventoryRet> itms)
        {
            try
            {
                if (itms != null)
                {
                    var itmcnt = 0;
                    List<Medicine> clst = null;
                    using (var ctx = new RMSModel())
                    {
                        clst = ctx.Item.OfType<Medicine>()
                            .Where(x => x.QBItemListID != null)
                            // .Where(x => x.ItemNumber == "6315")
                            .ToList();
                    }

                    Parallel.ForEach(itms, (item) => //.Where(x => x.ItemNumber == 6315)
                    {
                        if (itmcnt % 100 == 0)
                        {
                            Status = string.Format("{0} QBResults  Processed of {1}", itmcnt, itms.Count());
                        }
                        using (var ctx = new RMSModel())
                        {
                            QBInventoryItem i = ctx.QBInventoryItems.FirstOrDefault(x => x.ListID == item.ListID);
                            if (i == null)
                            {
                                i = new QBInventoryItem()
                                {
                                    ListID = item.ListID,
                                    ItemName = item.ItemName,
                                    ItemDesc2 = item.Desc1,
                                  
                                    DepartmentCode = "MISC",
                                   // ItemNumber = System.Convert.ToInt16(item.Name),
                                    TaxCode = item.TaxCode,
                                    Price = System.Convert.ToDouble(item.Price1),
                                    Quantity = System.Convert.ToDouble(item.QuantityOnHand),
                                    TrackingState = TrackingState.Added
                                   
                                };

                                ctx.QBInventoryItems.Add(i);
                            }

                            i.ItemName = item.ItemName;
                            i.ItemDesc2 = item.Desc1;
                            i.ListID = item.ListID;
                          
                            i.TaxCode = item.TaxCode;
                            //i.ItemNumber = System.Convert.ToInt16(item.Name);
                            i.Price = System.Convert.ToDouble(item.Price1);
                            i.Quantity = System.Convert.ToDouble(item.QuantityOnHand);

                            ctx.QBInventoryItems.AddOrUpdate(i);

                            Medicine itm = clst.FirstOrDefault(x => x.QBItemListID == i.ListID);
                            if (itm == null)
                            {
                                itm = new Medicine()
                                {
                                    DateCreated = DateTime.Now,
                                    SuggestedDosage = "Take as Directed by your Doctor",
                                    TrackingState = TrackingState.Added
                                };

                                ctx.Item.Add(itm);
                            }

                            if (itm != null)
                            {
                                itm.Description = i.ItemDesc2;
                                itm.Price = Convert.ToDecimal(i.Price);
                                itm.Quantity = Convert.ToDouble(i.Quantity);
                                itm.SalesTax = (i.TaxCode != null && i.TaxCode.ToUpper() == "TAX"
                                    ? (Decimal).15
                                    : (Decimal)0);
                                itm.QBItemListID = i.ListID;
                                itm.UnitOfMeasure = i.UnitOfMeasure;
                                itm.ItemName = i.ItemName;
                                itm.ItemNumber = i.ItemNumber.ToString();
                                itm.Size = i.Size;
                                ctx.Item.AddOrUpdate(itm);
                            }
                            ctx.SaveChanges();
                        }
                        itmcnt += 1;
                    });
                    //SaveDatabase();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void Notify(string token, object sender, NotificationEventArgs e)
        {
            MessageBus.Default.Notify(token, sender, e);
        }






        private Item item = null;

        public Item Item
        {
            get { return item; }
            set
            {
                if (item != null)
                {
                    item = value;
                    NotifyPropertyChanged(x => x.Item);
                }
            }
        }

        private ObservableCollection<TransactionsView> transactionList = null;

        public ObservableCollection<TransactionsView> TransactionList
        {
            get { return transactionList; }
            set
            {
                if (transactionList != value)
                {
                    transactionList = value;
                    NotifyPropertyChanged(x => x.TransactionList);
                }
            }
        }


        public Patient CreateNewPatient()
        {
            return new Patient() { TrackingState = TrackingState.Added };
        }

        public void SavePerson(Person patient)
        {
            try
            {
                using (var ctx = new RMSModel())
                {
                    ctx.Persons.AddOrUpdate(patient);
                    ctx.SaveChanges();
                    patient.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public List<TransactionBase> GetPatientTransactionList(Patient p)
        {
            using (var ctx = new RMSModel())
            {
                return
                    new List<TransactionBase>(
                        ctx.TransactionBase.OfType<Prescription>().Where(x => x.PatientId == p.Id).ToList());
            }
        }

        public List<TransactionBase> GetDoctorTransactionList(Doctor d)
        {
            using (var ctx = new RMSModel())
            {
                return
                    new List<TransactionBase>(
                        ctx.TransactionBase.OfType<Prescription>().Where(x => x.DoctorId == d.Id).ToList());
            }
        }


        public Doctor CreateNewDoctor(string searchtxt)
        {
            var d = CreateNewDoctor();
            SetNames(searchtxt, d);
            return d;
        }
        public Patient CreateNewPatient(string searchtxt)
        {
            var p = CreateNewPatient();
            SetNames(searchtxt, p);
            return p;
        }
        public Doctor CreateNewDoctor()
        {
            return new Doctor(){TrackingState = TrackingState.Added};
        }

        private void SetNames(string searchtxt, Person p)
        {
            var strs = searchtxt.Split(' ');
            p.FirstName = strs.FirstOrDefault();
            p.LastName = strs.LastOrDefault();
        }

        public void SaveTransaction()
        {
            try
            {
                if (TransactionData == null || TransactionData.TransactionEntries == null) return;

                if (SalesVM.Instance.TransactionData != null && SalesVM.Instance.TransactionData.GetType() == typeof(Prescription))
                {
                    var p = SalesVM.Instance.TransactionData as Prescription;
                    if (p.Doctor == null)
                    {
                        MessageBox.Show("Please Select a doctor");
                        return;
                    }
                    if (p.Patient == null)
                    {
                        MessageBox.Show("Please Select a Patient");
                        return;
                    }
                }

                //if (CurrentPharmacist != null)
                //{
                //    TransactionData.PharmacistId = CurrentPharmacist.Id;
                //}
                //else
                //{
                //    MessageBox.Show("Please Select Pharmacist! before continuing. ");
                //    return;
                //}
              

                using (var ctx = new RMSModel())
                {
                   try
                   {
                       var tstate = TransactionData.TrackingState;
                        SetTransactionNumber(TransactionData);
                        ctx.ApplyChanges(TransactionData);
                        ctx.SaveChanges();
                        if (tstate == TrackingState.Added)
                       {
                            SetTransactionNumber(TransactionData);
                            ctx.SaveChanges();
                       }
                       
                        
                        TransactionData.AcceptChanges();
                        
                    }
                    catch (Exception ex1)
                    {
                        if (!ex1.Message.Contains("Object reference not set to an instance of an object")) throw;
                    }
                        
                        TransactionData.TransactionId = TransactionData.TransactionId;
                        NotifyPropertyChanged(x => x.TransactionData);
                        var dbEntry = ctx.Entry(TransactionData);

                    if (dbEntry.State != EntityState.Deleted)
                    {
                        foreach (var itm in TransactionData.TransactionEntries)
                        {
                            itm.TransactionId =
                                TransactionData.TransactionEntries.FirstOrDefault(
                                    x => x.TransactionEntryId == itm.TransactionEntryId).TransactionId;
                            itm.TransactionEntryNumber = itm.TransactionEntryNumber;
                        }
                    }
                }
                
            }
            //catch (OptimisticConcurrencyException oce)
            //{
            //    foreach (var item in oce.StateEntries)
            //    {
            //        if (item.State == EntityState.Deleted)
            //        {
            //            rms.Detach(item.Entity);
            //        }
            //        else
            //        {
            //            rms.Refresh(RefreshMode.StoreWins, item);
            //            SaveTransaction();
            //        }
            //    }
            //    return false;
            //}
            //catch (UpdateException ue)
            //{


            //    foreach (ObjectStateEntry item in ue.StateEntries)
            //    {
            //        if (item.State == EntityState.Added || item.State == EntityState.Deleted)
            //        {
            //            rms.Detach(item.Entity);
            //        }
            //        else
            //        {
            //            rms.DeleteObject(item.Entity);
            //        }

            //        SaveTransaction();
            //        return false;
            //    }
            //    return true;
            //}
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void SetTransactionNumber(TransactionBase t)
        {
            if (t != null && (t.TransactionNumber == null || t.TransactionNumber == "0"))
            {
                t.TransactionNumber = t.TransactionId.ToString();
            }
        }

        private void CleanTransactionNavProperties(TransactionBase titm, RMSModel ctx)
        {
            try
            {
                var itm = titm as Prescription;
                if (itm != null)
                {
                    var dbEntityEntry = ctx.Entry(itm.Doctor);
                    if (dbEntityEntry != null &&
                        (dbEntityEntry.State != EntityState.Unchanged && dbEntityEntry.State != EntityState.Detached))
                    {
                        dbEntityEntry.State = EntityState.Unchanged;
                    }
                    var p = ctx.Entry(itm.Patient);
                    if (p != null &&
                        (p.State != EntityState.Unchanged && p.State != EntityState.Detached))
                    {
                        p.State = EntityState.Unchanged;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void SaveTransactionEntry(TransactionEntryBase itm, RMSModel ctx)
        {
            try
            {
                var dbEntityEntry = ctx.Entry(itm.Item);
                if (dbEntityEntry != null &&
                    (dbEntityEntry.State != EntityState.Unchanged && dbEntityEntry.State != EntityState.Detached))
                {
                    dbEntityEntry.State = EntityState.Unchanged;
                }
                // itm.Item = null;

                ctx.TransactionEntryBase.AddOrUpdate(itm);

            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private static Batch batch;

        public Batch Batch
        {
            get { return batch; }
            set
            {
                if (batch != value)
                {
                    batch = value;
                    NotifyPropertyChanged(x => x.Batch);
                }
            }

        }

        private static Station station;

        public Station Station
        {
            get { return station; }
            set
            {
                if (station != value)
                {
                    station = value;
                    NotifyPropertyChanged(x => x.Station);
                }
            }

        }

        private static Store store;

        public Store Store
        {
            get { return store; }
            set
            {
                if (store != value)
                {
                    store = value;
                    NotifyPropertyChanged(x => x.Store);
                }
            }

        }



        internal Prescription NewPrescription()
        {
            try
            {
                var trn = new Prescription()
                {
                    StationId = Station.StationId,
                    BatchId = Batch.BatchId,
                    Time = DateTime.Now,
                    CashierId = _cashier.Id,
                    StoreCode = Store.StoreCode,
                    TrackingState = TrackingState.Added
                };


                return trn;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public bool ServerMode { get; set; }
    }
}
