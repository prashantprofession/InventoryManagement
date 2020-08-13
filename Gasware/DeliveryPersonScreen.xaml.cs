using Gasware.Common;
using Gasware.ReportModels;
using Gasware.Service;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for DeliveryPersonScreen.xaml
    /// </summary>
    public partial class DeliveryPersonScreen : Window
    {
        private string _username;
        private IUnityContainer _container;
        private readonly IDeliveryPersonService _deliveryPersonService;

        public DeliveryPersonScreen(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
        }

        private void btnDeliveryPersonUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            DeliveryPersonViewModel deliveryPersonModel = (deliveryPersonGrid.SelectedItem as DeliveryPersonViewModel);
            DeliveryPersonUpdate deliveryPersonUpdate = new DeliveryPersonUpdate(_container, deliveryPersonModel, _username);
            deliveryPersonUpdate.Title = "Updating Delivery Person :" + deliveryPersonModel.DeliveryPersonId;
            deliveryPersonUpdate.AssignInterfaceValues();
            deliveryPersonUpdate.ShowDialog();
            deliveryPersonGrid.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
        }

        private void btnDeliveryPersonDelete_Clicked(object sender, RoutedEventArgs e)
        {
            DeliveryPersonViewModel deliveryPersonModel = (deliveryPersonGrid.SelectedItem as DeliveryPersonViewModel);
            var result = MessageBox.Show("Are you sure you want to delete the delivery person with id " +
              deliveryPersonModel.DeliveryPersonId + "?",
               "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.ToString() == "Yes")
            {
                // DeliveryPersonService deliveryPersonService = new DeliveryPersonService(_username);
                _deliveryPersonService.Delete(deliveryPersonModel);
                this.deliveryPersonGrid.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
                MessageBox.Show("Delivery person deleted successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnInsertDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            DeliveryPersonInsert deliveryPersonInsert = new DeliveryPersonInsert(_container, _username);
            deliveryPersonInsert.ShowDialog();            
            deliveryPersonGrid.ItemsSource = _deliveryPersonService.GetDeliveryPersons();
        }

        private void btnCloseDeliveryPerson_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            List<DeliveryPersonReportModel> deliveryPersons = _deliveryPersonService.GetDeliveryPersonReportModels();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(deliveryPersons);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(deliveryPersonGrid);
        }

    }
}
