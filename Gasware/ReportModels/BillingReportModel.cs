using System;

namespace Gasware.ReportModels

{
    public class BillingReportModel
    {
        public int BillingId { get; set; }
        public string Customer { get; set; }
        public string DeliveryPerson { get; set; }
        public int DeliveredFullCylinderQty { get; set; }
        public int ReceivedEmptyCylinderQty { get; set; }
        public double BilledAmount { get; set; }
        public string Product { get; set; }
        public double PaidAmount { get; set; }
        public DateTime BillingDate { get; set; }
        public int Invoice { get; set; }
        public decimal Rate { get; set; }
        public string Details { get; set; }
    }
}
