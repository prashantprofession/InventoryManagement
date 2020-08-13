using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ReportModels
{
    public class VendorReportModel
    {
        public int SupplierId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PinCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
