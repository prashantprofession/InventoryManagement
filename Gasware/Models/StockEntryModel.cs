using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.Models
{
    public class StockEntryModel
    {
        public int StockEntryId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public DeliveryPersonModel ReceivedBy { get; set; }
        public decimal UnitRate { get; set; }
        public double PaidAmount{ get; set; }
        public decimal CgstPaid { get; set; }
        public decimal SgstPaid { get; set; }
        public double Balance { get; set; }
        public double BilledAmount { get; set; }

    }
}
