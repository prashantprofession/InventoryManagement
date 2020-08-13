using System;

namespace Gasware.ReportModels
{
    public class InvoiceReportModel
    {
        public int InvoiceId { get; set; }

        public int Quantity { get; set; }

        public double RatePerQuantity { get; set; }

        public int BillNumber { get; set; }
        public string Customer { get; set; } 

        public string Product { get; set; }
        public string DeliveryPerson { get; set; }

        public double AmountWithoutGst { get; set; }

        public double Cgst { get; set; }

        public double Sgst { get; set; }

        public double TotalAmount { get; set; }

        public DateTime InvoiceDate { get; set; }
    }
}
