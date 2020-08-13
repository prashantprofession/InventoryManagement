using Gasware.Billing;
using Gasware.Models;
using Gasware.Repository;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Unity;

namespace Gasware.ViewModels
{
    public class LicenseViewModule
    {
        private readonly string _username;

        // private readonly IUnityContainer _container;
        private readonly UserRepository _userRepository;
        private readonly ProductRepository _productRepository;
        private readonly DailyReportRepository _dailyReportRepository;
        private readonly CompanyRepository _companyRepository;

        public LicenseViewModule(string username)
        {
            _username = username;
            _productRepository = new ProductRepository(username);
            _dailyReportRepository = new DailyReportRepository(username);
            _companyRepository = new CompanyRepository(username);
            _userRepository = new UserRepository(username);
            Billing = Visibility.Visible;
            StockEntry = Visibility.Visible;
            Vendor = Visibility.Visible;
            Customer = Visibility.Visible;
            Company = Visibility.Visible;
            DeliveryPerson = Visibility.Visible;
            Product = Visibility.Visible;
            LicenseExpiryString = GetLicenseExpiry();
            User = username == "admin" ? Visibility.Visible : Visibility.Hidden;
            LicenseButton = username == "superuser" ? Visibility.Visible : Visibility.Hidden;
            StockDetails = GetStockDetails();           
        }

        public Visibility Billing { get; set; }
        public Visibility StockEntry { get; set; }
        public Visibility Vendor { get; set; }
        public Visibility Customer { get; set; }
        public Visibility Company { get; set; }
        public Visibility DeliveryPerson { get; set; }
        public Visibility Product { get; set; }
        public Visibility User { get; set; }
        public Visibility LicenseButton { get; set; }
        public string LicenseExpiryString { get; set; }
        public List<string> StockDetails { get; set; }
        public bool EnableButtons { get; set; }
        public string GetLicenseExpiry()
        {
            UserModel userModel = _userRepository.GetUserByName("superuser");
            string decodedPwd = Password.GetDecodedPassword(userModel.Password);            
            decodedPwd = decodedPwd.Length == 10 ? "0" : decodedPwd.Substring(10,1);
            string licenseString = _companyRepository.GetLicenseExpiryDate(1).ToString("dd-MM-yyyy");
            licenseString = "Valid Till : " + licenseString  + " " + decodedPwd;
            return licenseString;
        }

        public bool EnableBillButton()
        {
            CompanyModel company = _companyRepository.Get(1);
            bool returnValue = true;
            if (company.CompanyId == 0 || company == null)
            {
                returnValue = false;
                MessageBox.Show("Company details are not added yet.Billing will not be available until you add.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            List<DailyReportModel> dailyReports = _dailyReportRepository.GetAllReports();
            if (dailyReports.Count == 0 || dailyReports.LastOrDefault().Balance <=0 )
            {
                returnValue = false;
                MessageBox.Show("Stock details are not added yet.Billing will not be available until you add.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            return returnValue;
        }

        public bool EnableDailyReportButton()
        {
            //DailyReportRepository dailyReportRepository  = new DailyReportRepository(_username);
            if (_dailyReportRepository.GetAllReports().Count == 0)
            {                              
                return false;
            }
            return true;
        }

        public bool EnableStockEntryButton()
        {
            //ProductRepository productRepository = new ProductRepository(_username);
            if (_productRepository.GetProducts().Count == 0)
            {                
                MessageBox.Show("Product details are not added yet.Billing will not be available until you add.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }
       
        public List<string> GetStockDetails()
        {
            DateTime dateTime = DateTime.Now;

            List<ProductModel> products = _productRepository.GetProducts();
            DailyReportModel previousDailyReport = null;
            DailyReportModel todaysDailyReport = null;
            List<String> stringList = new List<string>();
            foreach (ProductModel productModel in products )
            {
                previousDailyReport = _dailyReportRepository.GetDailyReportByDate(dateTime.AddDays(-1), productModel);
                todaysDailyReport = _dailyReportRepository.GetDailyReportByDate(dateTime, productModel);
                stringList.Add(productModel.Name.PadLeft(20)  +
                               previousDailyReport.Balance.ToString().PadLeft(15)   +
                               todaysDailyReport.Balance.ToString().PadLeft(15));
            }
            return stringList;
        }      

    }
}
