using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for EmailTemplateChange.xaml
    /// </summary>
    public partial class EmailTemplateChange : Window
    {
        private EmailTemplateModel _emailTemplateModel;
        private IEmailTemplateRepository _emailTemplateRepository;
        private string _username;
        private int _emailTemplateId;

        public EmailTemplateChange(IUnityContainer container, 
                                   string username, 
                                   EmailTemplateModel emailTemplateModel)
        {
            InitializeComponent();
            _emailTemplateModel = emailTemplateModel;
            _emailTemplateRepository = container.Resolve<IEmailTemplateRepository>();
            _username = username;
            _emailTemplateId = emailTemplateModel?.EmailTemplateId ?? 0;
            if (_emailTemplateId == 0)
                ClearFields();
            else
            {                
                AssignScreenFields(_emailTemplateModel);
            }
        }

        public void AssignScreenFields(EmailTemplateModel emailTemplateModel)
        {
            _emailTemplateModel = emailTemplateModel;
            _emailTemplateId = emailTemplateModel.EmailTemplateId;
            txtBody.Text = emailTemplateModel.Body;
            txtDisplayName.Text = emailTemplateModel.DisplayName;
            txtEmailId.Text = emailTemplateModel.EmailId;
            emailTypeCombo.Text = emailTemplateModel.EmailType;
            txtPassword.Password = emailTemplateModel.Password;
            txtSubject.Text = emailTemplateModel.Subject;                        
        }

        public void ClearFields()
        {
            txtBody.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtEmailId.Text = string.Empty;            
            txtPassword.Password = string.Empty;
            txtSubject.Text = string.Empty;

        }

  
        private EmailTemplateModel GetModelFromUIVendor()
        {
            return new EmailTemplateModel()
            {
                Body = txtBody.Text,
                DisplayName = txtDisplayName.Text,
                EmailId = txtEmailId.Text,
                EmailType = emailTypeCombo.Text,
                Password = txtPassword.Password.ToString(),
                Subject = txtSubject.Text,
                EmailTemplateId = _emailTemplateId
            };
        }

        private string ValidateInputFields()
        {
            string errorMessage = string.Empty;
            if (txtBody.Text.Length < 10 || string.IsNullOrEmpty(txtBody.Text))
                errorMessage += "Email Body should have atleast 10 characters" + Environment.NewLine;
            if (txtDisplayName.Text.Length < 6 || string.IsNullOrEmpty(txtDisplayName.Text))
                errorMessage += "Display name should have atleast 6 characters" + Environment.NewLine;
            if (txtEmailId.Text.Length < 11 || string.IsNullOrEmpty(txtEmailId.Text))
                errorMessage += "Invalid email id entered." + Environment.NewLine;
            else if (txtEmailId.Text.Substring(txtEmailId.Text.Length - 9).ToUpper() != "GMAIL.COM" )
                errorMessage += "Invalid G-Mail id entered." + Environment.NewLine;
            if (txtSubject.Text.Length < 6 || string.IsNullOrEmpty(txtSubject.Text))
                errorMessage += "Email Subject should have atleast 6 characters" ;
            if (string.IsNullOrEmpty(emailTypeCombo.Text) || emailTypeCombo.Text == "Select a value")
                errorMessage += "Email type should be selected.";
            return errorMessage;
        }

        
        private void btnVendorClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void btnEmailTemplateSave_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = ValidateInputFields();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }


            _emailTemplateModel = GetModelFromUIVendor();
            if (_emailTemplateModel.EmailTemplateId == 0 || _emailTemplateModel == null)
            {
                int vendorId = _emailTemplateRepository.Create(_emailTemplateModel);
                if (vendorId == 0)
                {
                    Utilities.ErrorMessage("Error occured while adding email template details");
                    return;
                }
                ClearFields();
            }
            else
            {
                _emailTemplateRepository.Update(_emailTemplateModel);
            }
            Utilities.SuccessMessage("Email template details saved successfully.");
        }

        private void btnEmailTemplateClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
