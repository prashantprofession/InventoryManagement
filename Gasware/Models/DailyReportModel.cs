using Gasware.ViewModels;
using System;

namespace Gasware.Models
{
    public class DailyReportModel
    {
        public int DailyReportId { get; set; }
        public ProductViewModel Product { get; set; } = new ProductViewModel();
        public int SoldQuantity { get; set; }
        public double SoldAmount { get; set; }
        public int ReceivedQuantity { get; set; }
        public double ReceivedAmount { get; set; }

        public DateTime TransactionDate { get; set; }

        public double Balance { get; set; }
    }
}
