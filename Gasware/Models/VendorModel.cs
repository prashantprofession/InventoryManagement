using System;


namespace Gasware.Models
{
    public class VendorModel
    {
        public int SupplierId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
