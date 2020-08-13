using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        CustomerModel Get(int id);

        List<CustomerModel> GetCustomers();

        int Create(CustomerModel customerModel);

        void Update(CustomerModel customerModel);
        void Delete(CustomerModel customerModel);
    }
}
