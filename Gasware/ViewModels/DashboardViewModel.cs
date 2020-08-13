using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ViewModels
{
    public class DashboardViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public double TotalStockInQuantity { get; set; }

        public double TotalBilledQuantity { get; set; }

        public double TotalReceivedEmptyQuantity { get; set; }

        public double OpeningBalance { get; set; }
        public double ClosingBalance { get; set; }

        public double TodaysSoldQty { get; set; }
        public double TodaysReceivedQty { get; set; }
    }
}
