using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ReportModels
{
    public class CustomerReportModel
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PinCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public int DiscountPercentage { get; set; }
        public int DiscountFlat { get; set; }
        public double DepositAmount { get; set; }
        public string GstNumber { get; set; }
        public string EmailId { get; set; }
    }
}
