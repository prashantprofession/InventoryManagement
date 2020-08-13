using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System.Collections.Generic;

namespace Gasware.Service.Interfaces
{
    public interface ICustomerService
    {
        CustomerViewModel Get(int id);
        List<CustomerViewModel> GetCustomers();
        List<CustomerReportModel> GetReportModelCustomers();
        void Update(CustomerViewModel customerViewModel);
        void Add(CustomerViewModel customerViewModel);
        void Delete(CustomerViewModel customerViewModel);

        CustomerViewModel GetViewModel(CustomerModel customerModel);
        CustomerModel GetDatabaseModel(CustomerViewModel customerViewModel);

    }
}
