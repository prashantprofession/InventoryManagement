using Gasware.Repository;

namespace Gasware.Common
{
    public static class CalculationProvider
    {
        public static double GetBillAmountWithoutGst(int quantity, decimal productRate, string username)
        {
            CompanyRepository companyRepository = new CompanyRepository(username);
            decimal cgstRate = companyRepository.GetTaxRate(1, "cgst");
            decimal sgstRate = companyRepository.GetTaxRate(1, "sgst");
            decimal totalBillAmount = quantity * productRate;
            double totalAmountWithoutGst = (double)(totalBillAmount - (
                                   (sgstRate + cgstRate) / 100 * totalBillAmount));
            return totalAmountWithoutGst;
        }

        public static double GetTaxAmount(double billAmount, string taxType, string username)
        {
            CompanyRepository companyRepository = new CompanyRepository(username);
            decimal taxRate = companyRepository.GetTaxRate(1, taxType);

           // double taxAmount = (double)((((100 + 2 * taxRate) / 100) * (taxRate / 100)) * billAmount);
            return ((double)taxRate / 100 * billAmount);
        }

        public static double GetCustomerInvoiceAmountWithoutGst(double billAmount, 
                                                                decimal cGstRate,
                                                                decimal sGstRate)
        {
            double cGstTaxAmount = GetCustomerInvoiceTax(billAmount, cGstRate, cGstRate + sGstRate);
            double sGstTaxAmount = GetCustomerInvoiceTax(billAmount, sGstRate, cGstRate + sGstRate);
            return (billAmount - cGstTaxAmount - sGstTaxAmount);
        }


        public static double GetCustomerInvoiceTax(double billAmount, decimal singleGstRate, decimal totalGstRate)
        {            
            decimal step1Calcuation = ((100 + totalGstRate) / 100); // 1.18
            decimal step2Calculation = singleGstRate / 100; // .09
            return System.Math.Round(billAmount / decimal.ToDouble(step1Calcuation) * 
                               decimal.ToDouble(step2Calculation), 2);             
        }


        public static decimal GetPercentageValue(decimal amount, decimal percentage)
        {
            return (amount / 100 * percentage);
        }

        public static double GetPercentageValue(double amount, double percentage)
        {
            return (amount / 100 * percentage);
        }

    }
}
