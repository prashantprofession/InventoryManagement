using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasware.ReportModels
{
    public class DailyTransactionReportModel
    {
        public int DailyReportId { get; set; }
        public string Product { get; set; }
        public int SoldQuantity { get; set; }
        public double SoldAmount { get; set; }
        public int ReceivedQuantity { get; set; }
        public double ReceivedAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Balance { get; set; }
    }
}
