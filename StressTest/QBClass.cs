using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using log4netWrapper;
using QS2QBPost.Properties;
using QuickBooks;
using RMSDataAccessLayer;
using SalesRegion;
using Timer = System.Timers.Timer;

namespace QS2QBPost
{
    public class QBClass
    {
        private static volatile QBClass instance;
        private static object syncRoot = new Object();
        private Timer postingTimer;
        private Timer downloadTimer;

        static QBClass()
        {
            Instance.postingTimer = new System.Timers.Timer(3000);
            Instance.postingTimer.Elapsed += Instance.OnTimeToPost;
            Instance.postingTimer.Enabled = true;

            Instance.downloadTimer = new System.Timers.Timer(Settings.Default.DownloadIntervalInMinutes*1000);
            //60 minutes * 1000 milliseconds
            Instance.downloadTimer.Elapsed += Instance.OnTimeToDownload;
            Instance.downloadTimer.Enabled = true;

        }

        public static QBClass Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new QBClass();
                    }
                }

                return instance;
            }
        }

        private async void OnTimeToDownload(object sender, ElapsedEventArgs e)
        {
            if (downloadTimer.Enabled == true) await DownloadFromQB();
        }

        private async Task DownloadFromQB()
        {


            var T = Task.Factory.StartNew(() => DownloadQBItems());
            T.Wait();


        }

        private async void OnTimeToPost(object sender, ElapsedEventArgs e)
        {
            if (postingTimer.Enabled == true) await PostToQB();
        }

        public async Task PostToQB()
        {
            try
            {
                Instance.postingTimer.Enabled = false;
                var lst = new List<TransactionBase>();
                using (var ctx = new RMSModel())
                {
                    int i = 0;
                    lst = ctx.TransactionBase.Where(x => x.Status == "ToBePosted")
                        .Include(x => x.TransactionEntries)
                        .Include("TransactionEntries.Item")
                        .Include(x => x.Cashier)
                        .ToList();
                }
                //while (keeprunning)
                foreach (var itm in lst)
                {
                    await Task.Run(() => Post(itm)).ConfigureAwait(false);
                }
                Instance.postingTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void Post(TransactionBase TransactionData)
        {
            try
            {

                IncludePrecriptionProperties(TransactionData);

                SalesReceipt s = new SalesReceipt();
                s.TxnDate = TransactionData.Time;
                s.TxnState = "1";
                s.Workstation = "02";
                s.StoreNumber = "1";
                s.SalesReceiptNumber = "123";
                s.Discount = "0";

                if (TransactionData == null || string.IsNullOrEmpty(TransactionData.TransactionNumber))
                {

                    //MessageBox.Show("Invalid Transaction Please Try again");
                    //TransactionData.Status = "Invalid Transaction Please Try again";
                    //rms.SaveChanges();
                    //return;
                }

                //TransPreZeroConverter tz = new TransPreZeroConverter();

                if (typeof (Prescription).IsInstanceOfType(TransactionData))
                {
                    Prescription p = TransactionData as Prescription;
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
                        p.TransactionNumber, doctor);
                }
                else
                {
                    s.Comments = "RX#:" + TransactionData.TransactionNumber;
                }

                //if (TransactionData == null || string.IsNullOrEmpty(TransactionData.TransactionNumber))
                //{
                //    rms.Refresh(RefreshMode.StoreWins, TransactionData);
                //    if (TransactionData == null || string.IsNullOrEmpty(TransactionData.TransactionNumber))
                //    {
                //        MessageBox.Show("Invalid Transaction Please Try again");
                //        return;
                //    }
                //}

                if (TransactionData != null)
                {
                    s.TrackingNumber = TransactionData.TransactionNumber;
                }
                s.Associate = "Dispensary";
                s.SalesReceiptType = "0";



                foreach (var item in TransactionData.TransactionEntries)
                {
                    if (item.Item.QBItemListID != null)
                    {

                        s.SalesReceiptDetails.Add(new SalesReceiptDetail
                        {
                            ItemListID = item.Item.QBItemListID,
                            QtySold = item.Quantity
                        }); //340 
                    }
                    else
                    {
                        ////MessageBox.Show("Please Link Quickbooks item to dispensary");
                        //TransactionData.Status = "Please Link Quickbooks item to dispensary";
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
                        TransactionData.ReferenceNumber = "QB#" + result.SalesReceiptNumber;
                        TransactionData.Status = "Posted";
                        ctx.TransactionBase.AddOrUpdate(TransactionData);
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

        private void IncludePrecriptionProperties(TransactionBase ptrn)
        {
            try
            {

                if (ptrn is Prescription)
                {

                    var pc = (ptrn as Prescription);
                    using (var ctx = new RMSModel())
                    {
                        pc.Doctor = ctx.Persons.OfType<Doctor>().FirstOrDefault(x => x.Id == pc.DoctorId);
                        pc.Patient = ctx.Persons.OfType<Patient>().FirstOrDefault(x => x.Id == pc.PatientId);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void DownloadQBItems(int days = 1)
        {
            try
            {

                Instance.downloadTimer.Enabled = false;

                // QBPOS pos = new QBPOS();
                List<ItemInventoryRet> itms = QBPOS.Instance.GetInventoryItemQuery(days);

                ProcessQBItems(itms);

                Instance.downloadTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }

        }

        private async Task ProcessQBItems(List<ItemInventoryRet> itms)
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
                        //if (itmcnt%100 == 0)
                        //{
                        //    ctx.SaveChanges(); //SaveDatabase();
                        //}
                        using (var ctx = new RMSModel())
                        {
                            QBInventoryItem i = ctx.QBInventoryItems.FirstOrDefault(x => x.ListID == item.ListID);
                            if (i == null)
                            {
                                i = new QBInventoryItem()
                                {
                                    ListID = item.ListID,
                                    ItemName = item.Desc1,
                                    ItemDesc2 = item.Desc2,
                                    Size = item.Size,
                                    DepartmentCode = "MISC",
                                    ItemNumber = System.Convert.ToInt16(item.ItemNumber),
                                    TaxCode = item.TaxCode,
                                    Price = System.Convert.ToDouble(item.Price1),
                                    Quantity = System.Convert.ToDouble(item.QuantityOnHand),
                                    UnitOfMeasure = item.UnitOfMeasure
                                };

                                ctx.QBInventoryItems.Add(i);
                            }

                            i.ItemName = item.Desc1;
                            i.ItemDesc2 = item.Desc2;
                            i.ListID = item.ListID;
                            i.Size = item.Size;
                            i.UnitOfMeasure = item.UnitOfMeasure;
                            i.TaxCode = item.TaxCode;
                            i.ItemNumber = System.Convert.ToInt16(item.ItemNumber);
                            i.Price = System.Convert.ToDouble(item.Price1);
                            i.Quantity = System.Convert.ToDouble(item.QuantityOnHand);

                            ctx.QBInventoryItems.AddOrUpdate(i);

                            Medicine itm = clst.FirstOrDefault(x => x.QBItemListID == i.ListID);
                            if (itm == null)
                            {
                                itm = new Medicine()
                                {
                                    DateCreated = DateTime.Now,
                                    SuggestedDosage = "Take as Directed by your Doctor"
                                };

                                ctx.Item.Add(itm);
                            }

                            if (itm != null)
                            {
                                itm.Description = i.ItemDesc2;
                                itm.Price = Convert.ToDecimal(i.Price);
                                itm.Quantity = Convert.ToDouble(i.Quantity);
                                itm.SalesTax = (i.TaxCode != null && i.TaxCode.ToUpper() == "VAT"
                                    ? (Decimal) .15
                                    : (Decimal) 0);
                                itm.QBItemListID = i.ListID;
                                itm.UnitOfMeasure = i.UnitOfMeasure;
                                itm.ItemName = i.ItemName;
                                itm.ItemNumber = i.ItemNumber.ToString();
                                itm.Size = i.Size;
                                ctx.Item.AddOrUpdate(itm);
                            }
                            ctx.SaveChanges();
                        }
                        // itmcnt += 1;
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

        internal async Task DownloadAllQBItems()
        {
            try
            {
                await Task.Run(() =>
                {
                    var t = QBPOS.Instance.GetAllInventoryQuery().Result;
                    ProcessQBItems(t);
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }
    }
}