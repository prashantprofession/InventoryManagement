using System;

namespace Gasware.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public AddressModel Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public int DiscountPercentage { get; set; }
        public int DiscountFlat { get; set; }
        public double DepositAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string GstNumber { get; set; }
        public string EmailId { get; set; }
    }
}
