using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using System;
using System.Net;
using System.Net.Mail;

namespace Gasware.Common
{
    public class EmailSendProvider
    {
        public void SendEmail(InvoiceModel invoiceModel, string billType)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceModel.Customer.EmailId))
                    return;
                CommonFunctions commonFunctions = new CommonFunctions();
                string companyLogoPath = @"C:\Prashant\Projects\DotNet\20200708_WPF\Gasware\assets\images\companyLogo.png";
                string vendorLogoPath = @"C:\Prashant\Projects\DotNet\20200708_WPF\Gasware\assets\images\vendorlogo.jpg"; 
                string invoiceBody = commonFunctions.GetInvoiceBody(invoiceModel, billType,invoiceModel.Product,
                                                    "companyLogo.png", "vendorlogo.jpg");     
                EmailTemplateRepository emailTemplateRepository = new EmailTemplateRepository("admin");
                EmailTemplateModel emailTemplateModel = emailTemplateRepository.GetEmailTemplateByType("Invoice");
                var fromAddress = new MailAddress(emailTemplateModel.EmailId, emailTemplateModel.DisplayName);
                var toAddress = new MailAddress(invoiceModel.Customer.EmailId , invoiceModel.Customer.Name);
                string fromPassword = emailTemplateModel.Password;
                string subject = emailTemplateModel.Subject;
                string body = emailTemplateModel.Body + 
                                Environment.NewLine + Environment.NewLine +
                                invoiceBody;                
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,                      
                })
                {
                    message.IsBodyHtml = true;
                    message.Attachments.Add(new Attachment(companyLogoPath));
                    message.Attachments.Add(new Attachment(vendorLogoPath));

                    smtp.Send(message);
                    Utilities.SuccessMessage("Invoice/Bill E-mail sent successfully...");
                }
            }
            catch(Exception ex)
            {
                Utilities.ErrorMessage("if this error occurs ensure following are taken care," + Environment.NewLine +
                    "1: You Email id and password are mentioned correctly in Email Template. " + Environment.NewLine +
                    "2: Enable less secure applications in you gmail account when prompted." + Environment.NewLine +
                    "3: If any other error below details below" + Environment.NewLine + ex.Message);
                return;
            }
        }

        public void SendBackupEmail(string attachmentFileName)
        {
            try
            {
                EmailTemplateRepository emailTemplateRepository = new EmailTemplateRepository("admin");
                EmailTemplateModel emailTemplateModel = emailTemplateRepository.GetEmailTemplateByType("BackUp");
                if (emailTemplateModel == null || string.IsNullOrEmpty(emailTemplateModel.EmailId))
                {
                    Utilities.ErrorMessage("Email template is either unavailable or emailid is unavailable.");
                    return;
                }
                var fromAddress = new MailAddress(emailTemplateModel.EmailId, emailTemplateModel.DisplayName);
                var toAddress = new MailAddress(emailTemplateModel.EmailId, emailTemplateModel.DisplayName);
                string fromPassword = emailTemplateModel.Password;
                string subject = emailTemplateModel.Subject;
                string body = emailTemplateModel.Body  ;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                })
                {
                    message.IsBodyHtml = true;
                    message.Attachments.Add(new Attachment(attachmentFileName));
                    smtp.Send(message);
                    Utilities.SuccessMessage("Backup E-mail sent successfully to emailid " + emailTemplateModel.EmailId);
                }
            }
            catch (Exception ex)
            {
                Utilities.ErrorMessage("if this error occurs ensure following are taken care," + Environment.NewLine +
                    "1: You Email id and password are mentioned correctly in Email Template. " + Environment.NewLine +
                    "2: Enable less secure applications in you gmail account when prompted." + Environment.NewLine +
                    "3: If any other error below details below" + Environment.NewLine + ex.Message);
                return;
            }
        }
    }
}
