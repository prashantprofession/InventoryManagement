using Gasware.Common;
using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for EmailTemplateScreen.xaml
    /// </summary>
    public partial class EmailTemplateScreen : Window
    {
        private readonly string _username;
        private readonly IUnityContainer _container;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private EmailTemplateModel _emailTemplateModel;

        public EmailTemplateScreen(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _emailTemplateRepository = _container.Resolve<IEmailTemplateRepository>();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            emailTemplatesDataGrid.ItemsSource = _emailTemplateRepository.GetEmailTemplates();
        }


        private void btnVendorClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btnCloseInsertUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExportToCsvClicked(object sender, RoutedEventArgs e)
        {
            EmailTemplateRepository emailTemplateRepository = new EmailTemplateRepository(_username);
            List<EmailTemplateModel> emailTemplateModels  = emailTemplateRepository.GetEmailTemplates();
            Gasware.Common.CommonFunctions commonFunctions = new Common.CommonFunctions();
            commonFunctions.ExportToCsvFile(emailTemplateModels);
        }

        private void btnPrintGridClicked(object sender, RoutedEventArgs e)
        {
            PrintFunction printFunction = new PrintFunction();
            printFunction.PrintGrid(emailTemplatesDataGrid);
        }

        private void btnNewInsertEmailTemplate_Click(object sender, RoutedEventArgs e)
        {
            EmailTemplateChange emailTemplateChange = new EmailTemplateChange(_container,
                                           _username, new EmailTemplateModel());
            emailTemplateChange.ShowDialog();
            SetDefaultValues();
        }

        private void btnCloseEmailTemplate_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEmailTemplateUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            _emailTemplateModel = (emailTemplatesDataGrid.SelectedItem as EmailTemplateModel);
            EmailTemplateChange emailTemplateChange = new EmailTemplateChange(_container, _username,
                                                _emailTemplateModel);
            emailTemplateChange.ShowDialog();
            SetDefaultValues();
        }

        private void btnEmailTemplateDelete_Clicked(object sender, RoutedEventArgs e)
        {
            EmailTemplateModel emailTemplateModel = (emailTemplatesDataGrid.SelectedItem as EmailTemplateModel);
            var result = Utilities.YesNoMessage("Are you sure you want to delete the Email Template with id " +
                                        emailTemplateModel.EmailTemplateId);
            if (result.ToString() == "Yes")
            {
                _emailTemplateRepository.Delete(emailTemplateModel);
                SetDefaultValues();
                Utilities.SuccessMessage("Email Template deleted successfully.");
            }
        }
    }
}
