using System;


namespace Gasware.ReportModels
{
    public class StockEntryReportModel
    {
        public int StockEntryId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public string ReceivedBy { get; set; }
        public decimal UnitRate { get; set; }
        public double PaidAmount { get; set; }
        public decimal CgstPaid { get; set; }
        public decimal SgstPaid { get; set; }
        public double Balance { get; set; }
        public double BilledAmount { get; set; }
    }
}
