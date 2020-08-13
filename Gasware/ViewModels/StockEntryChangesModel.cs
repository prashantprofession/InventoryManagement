using Gasware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ViewModels
{
    public class StockEntryChangesModel
    {
        public int StockEntryId { get; set; }

        public bool IsReceivedDateChanged { get; set; }

        public DateTime OldReceivedDate { get; set; }

        public DateTime NewReceivedDate { get; set; }
        public bool IsProductChanged { get; set; }

        public ProductModel OldProduct { get; set; }

        public ProductModel NewProduct { get; set; }

        public bool IsQuantityChanged { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public bool IsReceivedByChanged { get; set; }
        public DeliveryPersonModel OldReceivedBy { get; set; }
        public DeliveryPersonModel NewReceivedBy { get; set; }
    }
}
