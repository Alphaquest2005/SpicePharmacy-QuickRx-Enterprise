using System;
using System.Collections.Generic;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
//using System.Data.Entity.Validation;
using System.Linq;
using System.Data;
using System.IO;
using QBPOSFC3Lib;
using System.Windows;
using System.Runtime.InteropServices;

namespace QuickBooks
{
    public class QBPOS
    {
      static QBPOSSessionManager sessionManager = null;
        bool sessionBegun = false;
        bool connectionOpen = false;
        string qbposfile = Properties.Settings.Default.QBCompanyFile  ;

        private static object syncRoot = new Object();
        private static volatile QBPOS instance;
        public static QBPOS Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new QBPOS();
                    }
                }

                return instance;
            }
        }


        public QBPOS()
        {
            sessionManager = new QBPOSSessionManager();
        }

        private void CloseSession()
        {
            sessionManager.EndSession();
            sessionBegun = false;
            sessionManager.CloseConnection();
            connectionOpen = false;
        }

        private void BeginSession()
        {
            //sessionManager.OpenConnection("1", "Insight QBBulk");
            //short majorVersion;
            //short minorVersion;
            //ENReleaseLevel releaseLevel;
            //short releaseNumber;
            //sessionManager.GetVersion(out majorVersion, out minorVersion, out releaseLevel, out releaseNumber);
            try
            {
                sessionManager.OpenConnection("1", "Insight QBPOS");
                connectionOpen = true;
                sessionManager.BeginSession("Computer Name=user-think;Company Data=spice isle pharmacy;Version=9");//""
                // MessageBox.Show(sessionManager.GetCurrentCompanyFileName());
                // sessionManager.BeginSession("");
                sessionBegun = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
        }

        public IEnumerable<ItemInventoryRet> GetInventoryItemQuery(int days = 1)
        {
            
            

            IMsgSetRequest ItemInventoryRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
            ItemInventoryRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            ItemInventoryViewModel inventoryVM = new ItemInventoryViewModel();
            inventoryVM.BuildItemInventoryQueryRq(ItemInventoryRequestMsgSet, days);

            BeginSession();
            ////Send the request and get the response from QuickBooks
            IMsgSetResponse ItemInventoryResponseMsgSet = sessionManager.DoRequests(ItemInventoryRequestMsgSet);

            CloseSession();

            return inventoryVM.WalkItemInventoryQueryRs(ItemInventoryResponseMsgSet);

        }

        public void DoSalesReceipt()
        {
            //Create the message set request object to hold our request
            IMsgSetRequest SalesReceiptRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
            SalesReceiptRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            SalesReceiptViewModel SalesReceiptVM = new SalesReceiptViewModel();
            SalesReceiptVM.BuildSalesReceiptQueryRq(SalesReceiptRequestMsgSet);

            BeginSession();

            IMsgSetResponse SalesReceiptResponseMsgSet = sessionManager.DoRequests(SalesReceiptRequestMsgSet);

            CloseSession();

            SalesReceiptVM.WalkSalesReceiptQueryRs(SalesReceiptResponseMsgSet);

        }

        public SalesReceiptRet AddSalesReceipt(SalesReceipt salesreceipt)
        {
            //Create the message set request object to hold our request
            IMsgSetRequest SalesReceiptRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
            SalesReceiptRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            SalesReceiptViewModel SalesReceiptVM = new SalesReceiptViewModel();
            SalesReceiptVM.BuildSalesReceiptAddRq(SalesReceiptRequestMsgSet,
                                                 SalesReceiptVM.BuildSalesReceipt(salesreceipt));

            try
            {
                return GetQBSalesReceipt(salesreceipt, SalesReceiptRequestMsgSet, SalesReceiptVM);
            }
            catch (COMException ce)
            {
                MessageBox.Show("Connection to QuickBooks failed, please retry");
                return null;
            }
        }

        private SalesReceiptRet GetQBSalesReceipt(SalesReceipt salesreceipt, IMsgSetRequest SalesReceiptRequestMsgSet, SalesReceiptViewModel SalesReceiptVM)
        {
            BeginSession();

            IMsgSetResponse SalesReceiptResponseMsgSet = sessionManager.DoRequests(SalesReceiptRequestMsgSet);

            CloseSession();

            return SalesReceiptVM.WalkSalesReceiptAddRs(SalesReceiptResponseMsgSet, salesreceipt);
        }
  
       

        public void DoInventoryAdjustment()
        {
            //IMsgSetRequest InventoryAdjustmentRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
            //InventoryAdjustmentRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
            //InventoryAdjustmentViewModel AdjVM = new InventoryAdjustmentViewModel();
            //AdjVM.BuildInventoryAdjustmentQueryRq(InventoryAdjustmentRequestMsgSet);

            //BeginSession();

            //IMsgSetResponse InventoryAdjustmentResponseMsgSet = sessionManager.DoRequests(InventoryAdjustmentRequestMsgSet);

            //CloseSession();

            //AdjVM.WalkInventoryAdjustmentQueryRs(InventoryAdjustmentResponseMsgSet);

        }



       
    }
}