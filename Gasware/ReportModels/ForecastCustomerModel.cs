using System;

namespace Gasware.ReportModels
{
    public class ForecastCustomerModel
    {
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string PhoneNumber { get; set; }
        public decimal UnitRate { get; set; }
        public decimal CustomerRate { get; set; }
        public decimal TotalCustomerPrice { get; set; }
        public decimal TotalLatestPrice { get; set; }
        public int MininumNumberOfDays { get; set; }
        public DateTime LastSelectedDate { get; set; }

        public DateTime StockRequireDate { get; set; }
    }
}
