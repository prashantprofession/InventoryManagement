
using Gasware.Models;

namespace Gasware.ViewModels
{
    public class ProductViewModel
    {
        public int Productid { get; set; }
        public int Weight { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public decimal UnitRate { get; set; }
        public decimal CGstRate { get; set; }
        public decimal SGstRate { get; set; }
        public bool IsBillable { get; set; }
        public string IsBillableItem { get; set; }
        public bool IsExpense { get; set; }
        public string IsExpenseItem { get; set; }
    }
}
