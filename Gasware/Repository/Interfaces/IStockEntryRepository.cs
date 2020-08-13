using Gasware.Models;
using Gasware.ReportModels;
using Gasware.ViewModels;
using System;
using System.Collections.Generic;

namespace Gasware.Repository.Interfaces
{
    public interface IStockEntryRepository
    {
        StockEntryModel Get(int id);
        List<StockEntryModel> GetStockEntries();

        List<StockEntryReportModel> GetStockEntryReportModels();
        List<StockEntryModel> GetStockEntriesByDate(DateTime dateTime);

        StockEntryModel GetLatestStockForProduct(ProductViewModel productView);
        void Update(StockEntryModel stockEntryModel);

        void Update(StockEntryModel oldStockEntryModel, StockEntryModel newStockEntry);
        int Create(StockEntryModel stockEntryModel);
        void Delete(int id);
    }
}
