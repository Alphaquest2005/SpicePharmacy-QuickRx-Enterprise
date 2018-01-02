﻿
using System;
using System.Windows;
using log4netWrapper;


namespace QS2QBPost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var t = QBClass.Instance;
            Logger.Initialize();
            Logger.Log(LoggingLevel.Info, string.Format("Application Started - {0}", DateTime.Now));
        }

       

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
          await QBClass.Instance.PostToQB();
        }

        private void DownloadQBItemsBtn(object sender, RoutedEventArgs e)
        {
            QBClass.Instance.DownloadQBItems();
            MessageBox.Show("QuickBooks Inventory Downloaded");

        }

        private async void RefreshInventory(object sender, RoutedEventArgs e)
        {
           await QBClass.Instance.DownloadAllQBItems().ConfigureAwait(false);
            MessageBox.Show("QuickBooks Inventory Downloaded");
        }
    }
}

   
    

