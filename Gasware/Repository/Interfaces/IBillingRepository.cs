using Gasware.Models;
using System;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IBillingRepository
    {
        BillingModel GetBilling(int id);
        List<BillingModel> GetAllBillings();

        List<BillingModel> GetBillingsForDate(DateTime dateTime);
        int Update(BillingModel billingModel);
        int Create(BillingModel billingModel);
        void Delete(int id);
    }
}
