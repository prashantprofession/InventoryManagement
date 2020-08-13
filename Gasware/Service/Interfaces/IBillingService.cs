
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Service.Interfaces
{
    public interface IBillingService
    {
        BillingViewModel Get(int id);
        List<BillingViewModel> GetBillings();

        List<BillingReportModel> GetBillingReportModels();

        List<BillingReportModel> GetBillingReportModels(TransactionsFilterInputModel filterInputModel);
        List<BillingViewModel> GetBillingsForDate(DateTime dateTime);
        void Update(BillingViewModel billingViewModel);
        int Add(BillingViewModel billingViewModel);
        void Delete(int id);

        List<BillingViewModel> GetFilteredBills(TransactionsFilterInputModel transactionsFilter);
        BillingViewModel GetViewModel(BillingModel billingModel);

        BillingModel GetModelFromView(BillingViewModel billingViewModel);

    }
}
