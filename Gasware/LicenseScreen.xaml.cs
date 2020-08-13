using Gasware.Models;
using Gasware.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for LicenseScreen.xaml
    /// </summary>
    public partial class LicenseScreen : Window
    {
        private string _username;
        public LicenseScreen(string username)
        {
            InitializeComponent();
            _username = username;           
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            string selectedLicense = licenseCombo.Text;            
            CompanyRepository companyRepository = new CompanyRepository(_username);
            string expiryDate = companyRepository.UpdateLicense(1,selectedLicense);
            MessageBox.Show("License details updated successfully.Your license is valid till " + expiryDate,
                            "Information",MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
