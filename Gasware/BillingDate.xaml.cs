using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for BillingDate.xaml
    /// </summary>
    public partial class BillingDate : Window
    {
        public BillingDate()
        {
            InitializeComponent();            
        }

        private readonly string _username;
        private readonly IUnityContainer _container;
        private readonly IBillingService _billingService;

        public BillingDate(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            dpInputBillingDate.SelectedDate = DateTime.Now;
            _billingService = _container.Resolve<IBillingService>();
        }



        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = dpInputBillingDate.SelectedDate ?? DateTime.Now;
            this.Close();
            try
            {
                List<BillingViewModel> billingViewModels = 
                            _billingService.GetBillingsForDate(selectedDate);
                Billing_Screen billing_Screen = new Billing_Screen(_container, _username);
                billing_Screen.billingDataGrid.ItemsSource = billingViewModels;                
                billing_Screen.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while processing billing details",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
