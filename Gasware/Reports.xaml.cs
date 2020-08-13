using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {


        private readonly string _username;
       // private readonly DateTime _selectedDate;
        private readonly IUnityContainer _container;
        //private readonly IProductService _productService;
        //private readonly IDeliveryPersonService _deliveryPersonService;
        //private readonly ICustomerService _customerService;
        //private readonly IBillingService _billingService;
        //private readonly IStockEntryRepository _stockEntryRepository;
       // private TransactionsFilterInputModel filterModel;

    
            public Reports(IUnityContainer container, string username)
        {
            InitializeComponent();
            InitializeComponent();
            _username = username;           
            _container = container;
            //_productService = _container.Resolve<IProductService>();
            //_deliveryPersonService = _container.Resolve<IDeliveryPersonService>();
            //_customerService = _container.Resolve<ICustomerService>();
            //_billingService = _container.Resolve<IBillingService>();
            //_stockEntryRepository = _container.Resolve<IStockEntryRepository>();

        }

        private void customers_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deliveryperson_Click(object sender, RoutedEventArgs e)
        {

        }

        private void billing_Click(object sender, RoutedEventArgs e)
        {           
            //CommonFunctions commonFunctions = new CommonFunctions();
            //commonFunctions.ExportToCsvFile(_billingService.GetBillingReportModels());
        }

        private void products_Click(object sender, RoutedEventArgs e)
        {

        }

        private void vendor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void stockin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void invoice_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void dailyReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void users_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
