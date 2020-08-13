using Gasware.Models;


namespace Gasware.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public AddressModel Address { get; set; }
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
