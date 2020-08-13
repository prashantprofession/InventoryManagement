using System;

namespace Gasware.Models
{
    public class CompanyModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; } = new AddressModel();
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string License { get; set; }
        public decimal CentralGst { get; set; }
        public decimal StateGst { get; set; }
        public string ReportsPath { get; set; }
        public string GstNumber { get; set; }
        public string  AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string EmailId { get; set; }
        public string HSNNumber { get; set; }
        public string Password { get; set; }


    }
}
