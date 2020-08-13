using System;


namespace Gasware.ViewModels
{
    public class TransactionsFilterInputModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ProductViewModel ProductView { get; set; }

        public DeliveryPersonViewModel DeliveryPersonView { get; set; }
        public CustomerViewModel CustomerView { get; set; }
    }
}
