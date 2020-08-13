using Gasware.Models;
using Gasware.ReportModels;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using Gasware.Service.Interfaces;
using Gasware.ViewModels;
using System.Collections.Generic;
using Unity;

namespace Gasware.Service
{
    public class CustomerServiceProvider : ICustomerService
    {
        private ICustomerRepository customerRepository;
        private readonly string _username;

        public CustomerServiceProvider(IUnityContainer container, string username)
        {
            _username = username;
            customerRepository = container.Resolve<ICustomerRepository>();
            
        }

        public CustomerServiceProvider(string username)
        {
            _username = username;
        }
        public CustomerViewModel Get(int id)
        {
            if (customerRepository == null)
            {
                customerRepository = new CustomerRepository("admin");
            }

            CustomerModel customerModel = customerRepository.Get(id);
            if (customerModel == null)
            {
                return null;
            }
            return GetViewModel(customerModel);            
        }

        public List<CustomerViewModel> GetCustomers()
        {
            if (customerRepository == null)
            {
                customerRepository = new CustomerRepository("admin");
            }

            List<CustomerModel> customerModels = customerRepository.GetCustomers();
            List<CustomerViewModel>customerViewModels  = new List<CustomerViewModel>();
            foreach (CustomerModel customerModel in customerModels)
            {
                customerViewModels.Add(GetViewModel(customerModel));
            }
            return customerViewModels;
        }

        public List<CustomerReportModel> GetReportModelCustomers()
        {
            List<CustomerViewModel> customerViewModels = GetCustomers();
            List<CustomerReportModel> customerReportModels = new List<CustomerReportModel>();
            foreach(CustomerViewModel customerView in customerViewModels)
            {
                customerReportModels.Add(new CustomerReportModel()
                {
                    AddressId = customerView.Address.AddressId,
                    AddressLine1 = customerView.Address.AddressLine1,
                    AddressLine2 = customerView.Address.AddressLine2,
                    City = customerView.Address.City,
                    Country = customerView.Address.Country,
                    PinCode = customerView.Address.PinCode,
                    State = customerView.Address.State,
                    Street = customerView.Address.Street,
                    CustomerId = customerView.CustomerId,
                    DepositAmount = customerView.DepositAmount,
                    DiscountFlat = customerView.DiscountFlat,
                    DiscountPercentage = customerView.DiscountPercentage,
                    Location = customerView.Location,
                    Name = customerView.Name,
                    Price = customerView.Price,
                    PhoneNumber = customerView.PhoneNumber,
                    GstNumber = customerView.GstNumber,
                    EmailId = customerView.EmailId
                }) ;
            }
            return customerReportModels;
        }

        public CustomerViewModel GetViewModel(CustomerModel customerModel)
        {
            CustomerViewModel viewModel = new CustomerViewModel()
            {
                Address = customerModel.Address,
                CustomerId = customerModel.CustomerId,
                DepositAmount = customerModel.DepositAmount,
                DiscountFlat = customerModel.DiscountFlat,
                DiscountPercentage = customerModel.DiscountPercentage,
                Location = customerModel.Location,
                Name = customerModel.Name,
                PhoneNumber = customerModel.PhoneNumber,
                Price = customerModel.Price,
                GstNumber = customerModel.GstNumber,
                EmailId = customerModel.EmailId
            };
            return viewModel;
        }

        public CustomerModel GetDatabaseModel(CustomerViewModel customerViewModel)
        {
            CustomerModel customerModel = new CustomerModel()
            {
                Address = customerViewModel.Address,
                CustomerId = customerViewModel.CustomerId,
                DepositAmount = customerViewModel.DepositAmount,
                DiscountFlat = customerViewModel.DiscountFlat,
                DiscountPercentage = customerViewModel.DiscountPercentage,
                Location = customerViewModel.Location,
                Name = customerViewModel.Name,
                PhoneNumber = customerViewModel.PhoneNumber,
                Price = customerViewModel.Price,
                GstNumber = customerViewModel.GstNumber,
                EmailId = customerViewModel.EmailId
            };
            return customerModel;
        }


        public void Add(CustomerViewModel customerViewModel)
        {
            if (customerRepository == null)
            {
                customerRepository = new CustomerRepository("admin");
            }

            CustomerModel customerModel = GetDatabaseModel(customerViewModel);
            customerRepository.Create(customerModel);
        }

        public void Update(CustomerViewModel customerViewModel)
        {
            if (customerRepository == null)
            {
                customerRepository = new CustomerRepository("admin");
            }

            CustomerModel customerModel = GetDatabaseModel(customerViewModel);
            customerRepository.Update(customerModel);
        }

        public void Delete(CustomerViewModel customerViewModel)
        {
            if (customerRepository == null)
            {
                customerRepository = new CustomerRepository("admin");
            }

            CustomerModel customerModel = GetDatabaseModel(customerViewModel);
            customerRepository.Delete(customerModel);            
        }

    }
}
