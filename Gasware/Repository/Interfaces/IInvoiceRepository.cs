using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        InvoiceModel Get(int id);
        List<InvoiceModel> GetInvoices();
        List<InvoiceModel> GetInvoicesForDateRange(DateTime fromDate, DateTime toDate);
        List<InvoiceModel> GetInvoicesWithFilter(TransactionsFilterInputModel filterModel);
        List<InvoiceReportModel> GetInvoiceReportsWithFilter(TransactionsFilterInputModel filterModel);
        int Create(InvoiceModel invoice);
        void Update(InvoiceModel invoice);
        void Delete(int id);
    }
}
