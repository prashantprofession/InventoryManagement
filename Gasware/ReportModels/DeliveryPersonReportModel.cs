
namespace Gasware.ReportModels
{
    public class DeliveryPersonReportModel
    {
        public int DeliveryPersonId { get; set; }
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
