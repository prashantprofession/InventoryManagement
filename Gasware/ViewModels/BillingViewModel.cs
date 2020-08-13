using Gasware.Models;
using System;


namespace Gasware.ViewModels
{
    public class BillingViewModel
    {
        public int BillingId { get; set; }
        public CustomerViewModel Customer { get; set; }
        public DeliveryPersonViewModel DeliveryPerson { get; set; }
        public int DeliveredFullCylinderQty { get; set; }
        public int ReceivedEmptyCylinderQty { get; set; }
        public double BilledAmount { get; set; }
        public ProductViewModel Product { get; set; }
        public double PaidAmount { get; set; }
        public DateTime BillingDate { get; set; }
        public InvoiceModel Invoice { get; set; }
        public decimal Rate { get; set; }
        public string Details { get; set; }
        public bool IsDeleteButtonEnabled { get; set; }

    }
}
