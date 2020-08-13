using Gasware.Common;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for StockEntry.xaml
    /// </summary>
    public partial class StockEntry : Window
    {
        public StockEntry()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private readonly IProductRepository _productService;
        private readonly IDeliveryPersonRepository _deliveryPersonService;
        private readonly IStockEntryRepository _stockEntryRepository;

        public StockEntry(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _productService = _container.Resolve<IProductRepository>();
            _deliveryPersonService = _container.Resolve<IDeliveryPersonRepository>();
            _stockEntryRepository = _container.Resolve<IStockEntryRepository>();

        }

        private void btnNewInsertStockIn_Click(object sender, RoutedEventArgs e)
        {
            StockEntryInsert stockEntryInsert = new StockEntryInsert(_container, _username);
            stockEntryInsert.productCombo.ItemsSource = _productService.GetProducts();
            stockEntryInsert.receiversCombo.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
            stockEntryInsert.ShowDialog();
            stocksDataGrid.ItemsSource = _stockEntryRepository.GetStockEntries();
        }


        private void btnStockInUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            StockEntryModel stockEntry = (stocksDataGrid.SelectedItem as StockEntryModel);
            StockEntryUpdate stockEntryUpdate = new StockEntryUpdate(_container, stockEntry, _username);
            stockEntryUpdate.Title = "Updating Stock Id:" + stockEntry.StockEntryId;
            stockEntryUpdate.AssignInterfaceValues();
            stockEntryUpdate.ShowDialog();
            stocksDataGrid.ItemsSource = _stockEntryRepository.GetStockEntries();
        }

        private void btnStockInDelete_Clicked(object sender, RoutedEventArgs e)
        {
            StockEntryModel stockEntry = (stocksDataGrid.SelectedItem as StockEntryModel);
            var result = MessageBox.Show("Are you sure you want to delete the Stock with id " +
              stockEntry.StockEntryId + "?",
               "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.ToString() == "Yes")
            {
                _stockEntryRepository.Delete(stockEntry.StockEntryId);
                stocksDataGrid.ItemsSource = _stockEntryRepository.GetStockEntries();
                MessageBox.Show("Stock Entry deleted successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void btnNewCloseStockIn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCloseInsertStockIn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<StockEntryReportModel> stockEntryReportModels= _stockEntryRepository.GetStockEntryReportModels();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(stockEntryReportModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(stocksDataGrid);
        }
    }
}
