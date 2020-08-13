using Gasware.Repository;
using System;
using System.Collections.Generic;
using GWUtilities = Gasware.Database.Utilities;
using System.IO;
using System.Reflection;
using System.Linq;
using Gasware.Models;

namespace Gasware.Common
{
    public class CommonFunctions
    {


        public void ExportToCsvFile<T>(IEnumerable<T> items)
        {
            string fileName = GetReportsDirectory() + GetNewCsvFileName();


            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {

                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
            GWUtilities.InformationMessage("Data exported to file '" + fileName + "'");
        }


        public string GetReportsDirectory()
        {
            CompanyRepository companyRepository = new CompanyRepository("admin");
            return companyRepository.Get(1).ReportsPath + @"\";
        }

        public string GetNewCsvFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        }

       
        public string GetInvoiceBody(InvoiceModel invoiceModel, string invoiceType, ProductModel product,
                                     string companyLogoId, string vendorLogoId)
        {
            CompanyRepository companyRepository = new CompanyRepository("admin");
            CompanyModel company = companyRepository.Get(1);
            string companyLogoPath = "../assets/images/companyLogo.png";                     

            string emailBody =
                "<table style='height: 28px; width: 604px;'> " +
                "<tbody> " +
                "<tr style='height: 88px;'> " +
                "<td style='width: 84px; height: 88px;'> " +
                $@"<img src="" {companyLogoPath} "" alt='Company Logo' width='100' height='100' /> " +
                "</td> " +
                "<td style='width: 504px; height: 88px;'> " +
                "<table style='width: 509px;'> " +
                "<tbody> " +
                "<tr> " +
                "<td style='width: 499px; text-align: center;'> <span style='color: #808080;'> " +
                invoiceType +
                "</span> </td> " +
                "</tr> " +
                "<tr> " +
                "<td style='width: 499px; text-align: center;'> " +
                "<h2> <strong> INVOICE(BILL) </strong> </h2> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<p> <span style='color: #000000;'> </span> " + "</p> " +
                "<table style='width: 603px;'> " +
                "<tbody> " +
                "<tbody> " +
                "<tr style='height: 40px;'> " +
                "<td style='width: 80px; text-align: right; height: 40px;'> " +
                "<h3> " + "<span style='color: #808080;'> " + invoiceModel.Customer.Name +
                "</span> " + "</h3> " + "</td> " +
                "<td style='width: 400px; height: 40px;'> &nbsp; </td> " +
                "<td style='width: 150px; height: 40px;'> " + company.GstNumber +
                "</td> " +
                "</tr> " +
                "<tr style='height: 18px;'> " +
                "<td style='width: 80px; text-align: right; height: 18px;'>  Date Issued : </td> " +
                "<td style='width: 400px; height: 18px;'> <strong>" + invoiceModel.InvoiceDate + "</strong></td> " +
                "<td style='width: 150px; height: 18px;'> " +  company.Name + "</td> " +
                "</tr> " + "<tr style='height: 18px;'> " +
                "<td style='width: 80px; text-align: right; height: 18px;'> Invoice(Bill) No: </td> " +
                "<td style='width: 400px; height: 18px;'><strong>" + invoiceModel.InvoiceId.ToString().PadLeft(5,'0') +
                "</strong></td> <td style='width: 150px; height: 18px;'> " + 
                    company.Address.AddressLine1 + "," + company.Address.AddressLine2 + "</td> " +
                "</tr> <tr style='height: 18px;'> " +
                "<td style='width: 156px; text-align: right; height: 18px;'> Customer GST No: </td> " +
                "<td style='width: 303px; height: 18px;'> <strong>" + invoiceModel.Customer.GstNumber + " </strong></td> " +
                "<td style='width: 152px; height: 18px;'> " + company.Address.City + " </td> " +
                "</tr> <tr style='height: 18px;'> " +
                "<td style='width: 156px; height: 18px;'>  &nbsp;</td> " +
                "<td style='width: 303px; height: 18px;'> &nbsp; </td> " +
                "<td style='width: 152px; height: 18px;'> " + company.Address.PinCode + 
                "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<p> &nbsp;</p> " +
                "<table style='width: 609px;'> " +
                "<tbody> " +
                "<tr> " +
                "<td style='width: 115px; text-align: right;'> " + "<span style='color: #808080;'> " + "<strong> HSN No.</strong> " + "</span> " + "</td> " +
                "<td style='width: 160px; text-align: right;'> " + "<span style='color: #808080;'> " + "<strong>  PARTICULARS </strong> " + "</span> " + "</td> " +
                "<td style='width: 100px; text-align: right;'> " + "<span style='color: #808080;'> " + "<strong> QUANTITY </strong> " + "</span> " + "</td> " +
                "<td style='width: 100px; text-align: right;'> " + "<span style='color: #808080;'> " + "<strong> RATE </strong> </span> " + "</td> " +
                "<td style='width: 135px; text-align: right;'> " + "<span style='color: #808080;'> " + "<strong> AMOUNT </strong> " + "</span> " + "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<hr /> " +
                "<table style='width: 610px;'> " +
                "<tbody> " +
                "<tr style='height: 50px;'> " +
                "<td style='width: 115px; height: 29px;text-align: right;'> " + "<span style='color: #666699;'> " + company.HSNNumber + 
                            "</span> " + "</td> " +
                "<td style='width: 160px; text-align: right; height: 29px;'> " + "<span style='color: #666699;'> " + invoiceModel.Product.Name +
                            "</span> " + "</td> " +
                "<td style='width: 100px; text-align: right; height: 29px;'> " + "<span style='color: #666699;'> " + invoiceModel.Quantity.ToString() + 
                            "</span> " + "</td> " +
                "<td style='width: 100px; text-align: right; height: 29px;'> " + "<span style='color: #666699;'> " + invoiceModel.RatePerQuantity.ToString() +
                            "</span> " + "</td> " +
                "<td style='width: 135px; text-align: right; height: 29px;'> " + "<span style='color: #666699;'> " + invoiceModel.AmountWithoutGst.ToString() +
                "</span> " + "</td> " +
                "</tr> " +
                "<tr style='height: 18px;'> " +
                "<td style='width: 115px; height: 18px;'> &nbsp; </td> " +
                "<td style='width: 160px; text-align: right; height: 18px;'> &nbsp;</td> " +
                "<td style='width: 100px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> CGST </span> " + "</td> " +
                "<td style='width: 100px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> " + product.CGstRate.ToString() +
                " </span> " + "</td> " +
                "<td style='width: 135px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> " + invoiceModel.Cgst.ToString() +
                "</span> " + "</td> " +
                "</tr> " +
                "<tr style='height: 18px;'> " +
                "<td style='width: 115px; height: 18px;'> &nbsp;</td> " +
                "<td style='width: 160px; text-align: right; height: 18px;'> &nbsp;</td> " +
                "<td style='width: 100px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> SGST </span> " + "</td> " +
                "<td style='width: 100px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> " + product.SGstRate.ToString() +
                " </span> " + "</td> " +
                "<td style='width: 135px; text-align: right; height: 18px;'> " + "<span style='color: #666699;'> " + invoiceModel.Sgst.ToString() +
                            "</span> " + "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "</ br> " +
                "<table style='width: 610px;'> " +
                "<tbody> " +
                "<tr> " +
                "<td style='width: 261px;'> " + "<span style='color: #999999;'> Bank INFO </span> </td> " +
                "<td style='width: 331px; text-align: right;'> " + "<span style='color: #999999;'> TOTAL DUE </span> " + "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<hr /> " +
                "<table style='width: 610px;'> " +
                "<tbody> " +
                "<tr style='height: 16px;'> " +
                "<td style='width: 150px; text-align: right; height: 16px;'> " +
                "<p>  Account Number : </p> </td> " +
                "<td style='width: 310px; text-align: left; height: 16px;'> " + company.AccountNumber +
                 "</td> " +
                "<td style='width: 150px; text-align: right; height: 16px;'> <strong> Rs/-" + invoiceModel.TotalAmount.ToString() +
                            "</strong> " + "</td> " +
                "</tr> " +
                "<tr style='height: 18px;'> " +
                "<td style='width: 100px; text-align: right; height: 18px;'> IFSC Code : </td> " +
                "<td style='width: 360px; height: 18px;'> " + company.IFSCCode +
                            "</td> " +
                "<td style='width: 152px; height: 18px;'> &nbsp;</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<hr /> " +
                "<table style='width: 610px;'> " +
                "<tbody> " +
                "<tr> " +
                "<td style='width: 300px; text-align: ceter;'> &nbsp; &nbsp; Thank you!</td> " +
                "<td style='width: 160px; text-align: right;'> " + company.EmailId +
                            "</td> " +
                "<td style='width: 150px;'> " + company.PhoneNumber +  "</td> " +
                "</tr> " +
                "</tbody> " +
                "</table> " +
                "<p> " +
                //"<img src=\" " + vendorLogoPath + "\" alt='Vendor Logo' width='100' height='100' /> </p> " +
                $@"<img src=""https://photos.google.com/photo/AF1QipOprg7vbhl_LOhsCmndjHgLDe-q7a0EBvRcOaiH"" alt='Vendor Logo' width='100' height='100' />  </p> " +
                "<p> This invoice(bill) is generated by computer.Signature is not required</p>";
            return emailBody;
        }

        private static string base64String = null;
        public string ImageToBase64(string path)
        {            
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }
    }
}
