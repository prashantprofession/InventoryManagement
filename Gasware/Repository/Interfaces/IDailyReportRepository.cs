using Gasware.Database.gasgalleryDataSetTableAdapters;
using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Repository.Interfaces
{
    public interface IDailyReportRepository
    {
        DailyReportModel GetDailyReport(int id);
        List<DailyReportModel> GetAllReports();

        List<DailyReportModel> GetReportsForDateRange(DateTime fromDate, DateTime toDate, ProductViewModel product);

        List<DailyTransactionReportModel> GetPrintModels(DateTime fromDate, DateTime toDate, ProductViewModel product);

        int Update(DailyReportModel dailyReportModel);
        int Create(DailyReportModel dailyReportModel);

        int UpdateDailyReportForBill(BillingModel billing);

        int AddDailyReportForBill(BillingModel billing);

        int DeleteDailyReportForBill(BillingModel billing);

        int UpdateDailyReportForStock(StockEntryModel stock);

        int AddDailyReportForStock(StockEntryModel stock);

        int DeleteDailyReportForStock(StockEntryModel stock);

        DailyReportModel GetDailyReportByDate(DateTime dateTime, ProductModel product);
        void Delete(int id);
    }
}
