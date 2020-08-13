using Gasware.Common;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Service;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for DailyReportDateInput.xaml
    /// </summary>
    public partial class DailyReportDateInput : Window
    {
        private readonly string _username;

        public DailyReportDateInput()
        {
            InitializeComponent();
        }

        public DailyReportDateInput( string username)
        {
            InitializeComponent();
            _username = username;
            //dpInputFromDailyReportDate.SelectedDate = DateTime.Now;
            //dpInputToDailyReportDate.SelectedDate = DateTime.Now;
            ProductServiceProvider productService = new ProductServiceProvider("admin");
            productsCombo.ItemsSource = productService.GetProducts();
            refreshData(DateTime.MinValue, DateTime.MaxValue, null);
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            DateTime selectedFromDate = dpInputFromDailyReportDate.SelectedDate ?? DateTime.Now;
            DateTime selectedToDate = dpInputToDailyReportDate.SelectedDate ?? DateTime.Now;
            ProductViewModel productViewModel = (productsCombo.SelectedItem as ProductViewModel);
            refreshData(selectedFromDate, selectedToDate, productViewModel);
        }

        private void refreshData(DateTime fromDate, DateTime toDate, ProductViewModel product)
        {
            DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
            dailyReportDataGrid.ItemsSource = 
                    dailyReportRepository.GetReportsForDateRange(fromDate, toDate, product);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dailyReportDataGrid_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {

            DailyReportRepository dailyReportRepository = new DailyReportRepository(_username);
            DateTime fromDate = dpInputFromDailyReportDate.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = dpInputToDailyReportDate.SelectedDate ?? DateTime.MaxValue;
            List<DailyTransactionReportModel> dailyReports =
                    dailyReportRepository.GetPrintModels(fromDate,toDate, 
                                                         (productsCombo.SelectedItem as ProductViewModel));
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(dailyReports);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(dailyReportDataGrid);
        }
    }
}
