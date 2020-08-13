
using System;

namespace Gasware.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }

        public int Quantity { get; set; }

        public double RatePerQuantity { get; set; }

        public BillingModel Billing { get; set; }
        public CustomerModel Customer { get; set; } = new CustomerModel();

        public ProductModel Product { get; set; } = new ProductModel();
        public DeliveryPersonModel DeliveryPerson { get; set; } = new DeliveryPersonModel();

        public double AmountWithoutGst { get; set; }

        public double Cgst { get; set; }

        public double Sgst { get; set; }

        public double TotalAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public bool IsBillRequired { get; set; } = true;
    }
}
