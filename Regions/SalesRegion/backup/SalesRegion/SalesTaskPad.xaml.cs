﻿using RMSDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using log4netWrapper;


namespace SalesRegion
{

    /// <summary>
    /// Interaction logic for SalesTaskPad.xaml
    /// </summary>
    public partial class SalesTaskPad : UserControl
    {
        private static readonly SalesTaskPad _instance;

        static SalesTaskPad()
        {
            _instance = new SalesTaskPad();
        }

        public static SalesTaskPad Instance
        {
            get { return _instance; }
        }

        public SalesTaskPad()
        {
            InitializeComponent();
            this.DataContextChanged += SalesTaskPad_DataContextChanged;


        }

        private void SalesLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReBindItemEditor();

        }


        private void SalesTaskPad_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SearchBox != null) SearchBox.Focus();
            ReBindTranStatusTxt();
        }

        private void ReBindTranStatusTxt()
        {
            Binding myBinding = new Binding("Status");
            myBinding.Source = SalesVM.Instance.TransactionData;
            if (SalesPad != null)
            {
                var frameworkElement = (FrameworkElement) SalesPad.FindName("TransStatusTxt");
                if (frameworkElement != null)
                    if (TextBlock.TextProperty != null) frameworkElement.SetBinding(TextBlock.TextProperty, myBinding);
            }
        }


        private void ReBindItemEditor()
        {
            try
            {
                ReBindTranStatusTxt();
                Binding myBinding = new Binding("SelectedItem");
                myBinding.Source = SalesView.SalesLst;
                myBinding.Mode = BindingMode.OneWay;

                if (ContentProperty != null)
                    ((FrameworkElement) SalesPad.FindName("ItemEditor")).SetBinding(ContentProperty, myBinding);
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }





        public TransactionBase Transaction
        {
            get { return SalesVM.Instance.TransactionData; }
        }


        private void SalesPad_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {

                if (e == null) return;
                var uie = e.OriginalSource as Control;

                if (uie == null) uie = SearchBox as Control;
                if (uie.Name == "PART_FilterBox")
                {
                    if (e.Key == Key.Enter) //pkey == Key.Enter &&
                    {
                        // SearchBox.RaiseFilterEvent();
                        if (SearchListCtl.Items.Count == 1)
                            SearchListCtl.SelectedIndex = 0;
                        var textBox = uie as TextBox;
                        if (textBox != null) textBox.Text = "";
                        //e.Handled = true;
                        MoveToNextControl(uie);
                    }

                    return;
                }

                //if (e.Key == Key.Enter)
                //{
                //    // e.Handled = true;
                //    MoveToNextControl(uie);
                //}
                pkey = e.Key;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        public void MoveToNextControl(object sender)
        {
            try
            {
                UIElement uie = sender as UIElement;
                uie.MoveFocus(
                    new TraversalRequest(
                        FocusNavigationDirection.Next));
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void FilterControl_Direction_1(object sender, DirectionEventArgs e)
        {
            try
            {
                if (e.Direction == DirectionEnum.Down)
                {
                    if (SearchListCtl != null) SearchListCtl.SelectedIndex += 1;
                }
                if (e.Direction == DirectionEnum.Up && SearchListCtl.SelectedIndex > -1)
                {
                    SearchListCtl.SelectedIndex -= 1;
                }
                if (e.Direction == DirectionEnum.Right)
                {

                    MoveToNextControl(sender);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private Key pkey;

        private void SearchBox_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    DeleteTransactionEntry();
                }

                if (SearchListCtl.Items.Count == 1)
                    SearchListCtl.SelectedIndex = 0;

                if (e.Key == Key.Enter)
                {

                    LocalProcesItem(SearchListCtl.SelectedItem);
                    MoveToNextControl(sender);
                }


                pkey = e.Key;
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void DeleteTransactionEntry()
        {
            try
            {
                if (SalesVM.Instance.TransactionData != null &&
                    SalesVM.Instance.TransactionData.CurrentTransactionEntry != null)
                {
                    SalesVM.Instance.DeleteTransactionEntry<TransactionEntryBase>(
                        SalesVM.Instance.TransactionData.CurrentTransactionEntry);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void LocalProcesItem(object itm)
        {
            try
            {
                if (ItemEditor == null) return;
                if (itm == null) return;

                if (edititem == true)
                {
                    ItemEditor.Content = itm;
                    edititem = false;
                    return;
                }


                if (itm.GetType() == typeof (RMSDataAccessLayer.SearchItem))
                {
                    switch (((ISearchItem) itm).DisplayName)
                    {

                        case "Add Patient":


                            Patient p = SalesVM.Instance.CreateNewPatient(SearchBox.Text);

                            ItemEditor.Content = p;

                            break;
                        case "Add Doctor":
                            Doctor d = SalesVM.Instance.CreateNewDoctor(SearchBox.Text);

                            ItemEditor.Content = d;
                            break;

                        default:
                            ItemEditor.Content = ((RMSDataAccessLayer.SearchItem) itm).SearchObject;
                            break;
                    }
                }



                else
                {
                    if (showPatientPrescriptions == true)
                    {
                        if (itm.GetType() == typeof (RMSDataAccessLayer.Patient))
                        {
                            var lst = SalesVM.Instance.GetPatientTransactionList(itm as Patient);
                            ItemEditor.Content = lst;
                            showPatientPrescriptions = false;
                        }

                        if (itm.GetType() == typeof (RMSDataAccessLayer.Doctor))
                        {
                            var lst = SalesVM.Instance.GetDoctorTransactionList(itm as Doctor);
                            ItemEditor.Content = lst;
                            showPatientPrescriptions = false;
                        }
                    }
                    else
                    {
                        if (SalesVM.Instance != null) SalesVM.Instance.ProcessSearchListItem(itm);
                        MoveToNextControl(ItemEditor);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private SalesView _SalesView;

        public SalesView SalesView
        {
            get { return _SalesView; }
            set
            {
                _SalesView = value;
                SalesView.SalesLst.SelectionChanged += SalesLst_SelectionChanged;
            }
        }



        private Timer queryTimer;

        private void RevokeQueryTimer()
        {
            if (queryTimer != null)
            {
                queryTimer.Stop();
                queryTimer.Elapsed -= queryTimer_Tick;
                queryTimer = null;
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(HideSearchList));
            }
        }

        private void RestartQueryTimer()
        {
            // Start or reset a pending query
            if (queryTimer == null)
            {
                queryTimer = new Timer {Enabled = true, Interval = 300};
                queryTimer.Elapsed += queryTimer_Tick;
            }
            else
            {
                queryTimer.Stop();
                queryTimer.Start();
            }
        }

        private void queryTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer so it doesn't fire again unless rescheduled
            RevokeQueryTimer();

            // Perform the query
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (SalesVM.Instance != null) if (SearchBox != null) SalesVM.Instance.GetSearchResults(SearchBox.Text);
                ShowSearchList();
            }));
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (e != null && e.Changes != null)
                {
                    //if (SalesVM.Instance != null) if (SearchBox != null) SalesVM.Instance.GetSearchResults(SearchBox.Text);

                    //ShowSearchList();
                    RestartQueryTimer();

                }
                else
                {
                    HideSearchList();
                    RevokeQueryTimer();
                }

            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }


        private void HideSearchList()
        {
            if (SearchListCtl != null)
            {
                SearchListCtl.Visibility = System.Windows.Visibility.Collapsed;
                SearchListCtl.Focusable = false;
                SearchListCtl.SelectedIndex = -1;
            }
        }

        private void ShowSearchList()
        {
            if (SearchListCtl != null && !string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchListCtl.Visibility = System.Windows.Visibility.Visible;
                SearchListCtl.Focusable = true;
                if (SearchListCtl.Items.Count == 1)
                    SearchListCtl.SelectedIndex = 0;
            }
        }



        private void TextBox_GotKeyboardFocus_1(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null) textBox.SelectAll();
        }

        public async void PrintBtn_Click_1(object sender, RoutedEventArgs e)
        {


            if (ReceiptCol != null && ReceiptCol.Width == new GridLength(0))
            {
                // unhide the colums to print
                ReceiptCol.Width = new GridLength(400);
                ReceiptGrd.UpdateLayout();

                await Print().ConfigureAwait(false);

                ReceiptCol.Width = new GridLength(0);
                //hide it back
            }
            else
            {

                await Print().ConfigureAwait(false);

            }

        }


        private async Task Print()
        {
            try
            {
                if (SalesVM.Instance != null &&
                    (SalesVM.Instance.TransactionData is QuickPrescription ||
                     SalesVM.Instance.TransactionData is Prescription))
                {

                    ListView plist = Common.FindChild<ListView>(ReceiptGrd, "PrescriptionEntriesRptLst");
                    // (ListView)this.FindName("PrescriptionEntriesRptLst");
                    if (plist != null)
                        await PrintListItems(plist).ConfigureAwait(false);
                }

                else
                {
                    if (SalesVM.Instance != null) SalesVM.Instance.Print(ref ReceiptGrd);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private async Task PrintListItems(ListView plist)
        {
            try
            {
                if (plist == null) return;
                dynamic lst;
                if (plist.SelectedItems.Count == 0)
                {
                    lst = plist.Items;
                }
                else
                {
                    lst = plist.SelectedItems;
                }
                await Task.Run(() =>
                {
                    foreach (var item in lst)
                    {
                        if (plist.ItemContainerGenerator != null)
                        {
                            FrameworkElement fi =
                                (FrameworkElement) plist.ItemContainerGenerator.ContainerFromItem(item);

                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                SalesVM.Instance.Print(ref fi);
                            }));

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }



        private void NewTransaction(object sender, RoutedEventArgs e)
        {
            if (SalesVM.Instance != null) SalesVM.Instance.TransactionData = SalesVM.Instance.NewPrescription();
        }

        private bool focusswitch;

        private void TransStatusTxt_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if (focusswitch == true)
            {
                ReBindTranStatusTxt();
            }

            focusswitch = !focusswitch;

        }


        private int TemplateHeight = 192;

        private void PrescriptionEntriesRptLst_LayoutUpdated(object sender, EventArgs e)
        {
            if (SalesView != null &&
                (SalesVM.Instance != null &&
                 (SalesVM.Instance.TransactionData != null && SalesView.SalesPadState == SalesPadTransState.Receipt)))
                if (SalesVM.Instance.TransactionData.TransactionEntries != null)
                {
                    var frameworkElement = (FrameworkElement) this.FindName("ReceiptGrd");
                    if (frameworkElement != null)
                        frameworkElement.Height = TemplateHeight*
                                                  SalesVM.Instance.TransactionData.TransactionEntries.Count;
                }
        }


        private void SearchListCtl_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (SearchListCtl != null) LocalProcesItem(SearchListCtl.SelectedItem);
            MoveToNextControl(sender);
            if (SearchBox != null) SearchBox.Text = "";
            HideSearchList();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchListCtl != null && SearchListCtl.Visibility == System.Windows.Visibility.Visible)
            {
                HideSearchList();
            }
            else
            {
                ShowSearchList();
            }
        }

        private void NextEntry_Click(object sender, RoutedEventArgs e)
        {
            if (SalesView != null)
            {
                SalesView.pkey = Key.Down;
                SalesView.padPos = SalesRegion.SalesView.PadPosition.Middle;
                SalesView.GoToNextTransactionEntry();
            }
        }

        private void DeleteTranBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteTransactionEntry();
        }

        private void BackBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SalesView != null) SalesView.GotoPreviousSalesStep();
        }

        private void NextBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SalesView != null) SalesView.GotoNextSalesStep(Key.Right);
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (SalesVM.Instance != null)
                {
                    var transactionBase = e.AddedItems[0] as TransactionBase;
                    if (transactionBase != null)
                        SalesVM.Instance.GoToTransaction(transactionBase.TransactionId);

                }
                ReBindItemEditor();
            }
        }

        private void SavePatientBtn_Click(object sender, RoutedEventArgs e)
        {

            if (ItemEditor.Content is Person)
            {
                SalesVM.Instance.SavePerson((Person) ItemEditor.Content);
            }
            //SalesVM.Instance.SaveTransaction();

            LocalProcesItem((Person) ItemEditor.Content);
            ReBindItemEditor();
        }

        private bool edititem = false;

        private void EditItemTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            edititem = true;
        }

        private bool showPatientPrescriptions = false;

        private void PatientPrescriptionBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showPatientPrescriptions = true;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemEditor.Content = null;
            ReBindItemEditor();
            // SalesVM.Instance.DiscardChanges();
        }

        private async void PostToQB_Click(object sender, RoutedEventArgs e)
        {
           await Task.Run(() => SalesVM.Instance.PostQBSale()).ConfigureAwait(false);
          //  NextBtn_MouseLeftButtonDown(sender, null);
        }



        private async void PrintBtn_RightClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListView plist = Common.FindChild<ListView>(ReceiptGrd, "PrescriptionEntriesRptLst");
                // (ListView)this.FindName("PrescriptionEntriesRptLst");
                if (plist != null)
                {
                    plist.SelectAll();
                    PrintBtn_Click_1(sender, e);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView plist = Common.FindChild<ListView>(ReceiptGrd, "PrescriptionEntriesRptLst");
            // (ListView)this.FindName("PrescriptionEntriesRptLst");
            if (plist != null)
            {
                plist.SelectedItems.Clear();
            }
        }

        private void PharmacistCbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e != null && e.AddedItems.Count > 0)
                {
                    Cashier c = (e.AddedItems[0] as Cashier);
                    if (c != null && SalesVM.Instance.TransactionData != null)
                        SalesVM.Instance.TransactionData.PharmacistId = c.Id;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggingLevel.Error, ex.Message + ex.StackTrace);
                throw ex;
            }
        }

        private void SetDosage(object sender, MouseButtonEventArgs e)
        {
            var m = SalesVM.Instance.TransactionData.CurrentTransactionEntry.Item as Medicine;
            if (m != null)
            {
                using (var ctx = new RMSModel())
                {
                    m.SuggestedDosage = SalesVM.Instance.TransactionData.CurrentTransactionEntry.Dosage;
                    ctx.Item.AddOrUpdate(m);
                    ctx.SaveChanges();
                    MessageBox.Show("Suggested Dosage Saved");
                }
            }
        }
    }


}
