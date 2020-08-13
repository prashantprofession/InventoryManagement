using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Vendor.xaml
    /// </summary>
    public partial class Vendor : Window
    {
        private string _username;
        private readonly IUnityContainer _container;
        //private static int _vendorId = 0;
        //private static int _addressId = 0;
        //private static VendorModel vendorModel = new VendorModel();
        private VendorViewModel _vendorModel = new VendorViewModel();

        private readonly IVendorService _vendorService;

        public Vendor()
        {
            InitializeComponent();
        }
        public Vendor(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            // _vendorId = 1;
            _vendorService = _container.Resolve<IVendorService>();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            vendorsDataGrid.ItemsSource = _vendorService.GetVendors();
        }



        private void btnVendorSave_Click(object sender, RoutedEventArgs e)
        {
        }           

        private void btnVendorClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVendorUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            _vendorModel = (vendorsDataGrid.SelectedItem as VendorViewModel);
            VendorChange vendorChange = new VendorChange(_container, _username,
                                                _vendorModel.SupplierId);
            vendorChange.ShowDialog();
            SetDefaultValues();
        }

        private void btnVendorDelete_Clicked(object sender, RoutedEventArgs e)
        {
            VendorViewModel vendorViewModel = (vendorsDataGrid.SelectedItem as VendorViewModel);
            var result = Utilities.YesNoMessage("Are you sure you want to delete the User with id " +
                                        vendorViewModel.SupplierId);
            if (result.ToString() == "Yes")
            {
                _vendorService.Delete(vendorViewModel);
                vendorsDataGrid.ItemsSource = _vendorService.GetVendors();
                Utilities.SuccessMessage("Dealer/Supplier/Vendor deleted successfully.");
            }
        }

        private void btnNewInsertVendor_Click(object sender, RoutedEventArgs e)
        {           
            VendorChange vendorChange = new VendorChange(_container, _username,
                                                0);
            vendorChange.ShowDialog();
            SetDefaultValues();
        }

        private void btnCloseInsertUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<VendorReportModel> vendorReportModels= _vendorService.GetVendorReportModels();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(vendorReportModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(vendorsDataGrid);
        }
    }

}
