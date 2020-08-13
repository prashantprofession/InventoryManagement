using Gasware.ViewModels;
using System;

namespace Gasware.Models
{
    public class BillingModel
    {
        public int BillingId { get; set; }
        public CustomerViewModel Customer { get; set; } = new CustomerViewModel();
        public DeliveryPersonViewModel DeliveryPerson { get; set; } = new DeliveryPersonViewModel();
        public InvoiceModel Invoice { get; set; } = new InvoiceModel();
        public int DeliveredFullCylinderQty { get; set; }
        public int ReceivedEmptyCylinderQty { get; set; }
        public double BilledAmount { get; set; }
        public ProductViewModel Product { get; set; } = new ProductViewModel();
        public double PaidAmount { get; set; }
        public DateTime BillingDate { get; set; }
        public decimal Rate { get; set; }
        public string Details { get; set; }
    }
}
