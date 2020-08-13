using Gasware.Repository.Interfaces;
using System;
using System.Windows;
using Unity;
using Gasware.Models;
using Gasware.Repository;
using System.Windows.Input;
using Gasware.Database;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IUserRespository _userRespository;
        private readonly IUnityContainer _container;

        public MainWindow(IUnityContainer container)
        {
            InitializeComponent();
            _container = container;
            _userRespository = container.Resolve<IUserRespository>();
            txtUsername.Focus();
        }



        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ExecuteLogin();
        }

        private void ExecuteLogin()
        {
            string errorMessage = AreInputFieldsValid();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.ErrorMessage(errorMessage);
                return;
            }
            else
            {
                try
                {
                    string dbPwdString = string.Empty;
                    UserModel dbUserModel = _userRespository.GetUserByName(txtUsername.Text);
                    if (dbUserModel == null)
                    {
                        Utilities.ErrorMessage("User is Invalid.Retry with correct credentials.");
                    }
                    else
                    {
                        dbPwdString = Password.GetDecodedPassword(dbUserModel.Password);
                    }

                    if (pwdPassword.Password.ToString().Equals(dbPwdString,
                                StringComparison.OrdinalIgnoreCase))
                    {
                        CompanyRepository companyRepository = new CompanyRepository(dbUserModel.Name);
                        DateTime expiryDate = companyRepository.GetLicenseExpiryDate(1);
                        if (expiryDate < DateTime.Now)
                        {
                            Utilities.InformationMessage("License has expired for this product.");
                            this.Close();
                            return;
                        }
                        else if (DateTime.Now < expiryDate && expiryDate.Subtract(DateTime.Now).TotalDays <= 15)
                        {
                            string noOfDays = ((int)(expiryDate.Subtract(DateTime.Now).TotalDays)).ToString();
                            Utilities.InformationMessage("Your license will expire in " + noOfDays + " days.");
                        }

                        MessageBox.Show("You are authorized User.Congratulations!!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ChangeSuperUserPassword(dbUserModel);
                        Dashboard dashboard = new Dashboard(dbUserModel.Name, _container);
                        if (txtUsername.Text == "admin" || txtUsername.Text == "superuser")
                        {
                            dashboard.users.Visibility = Visibility.Visible;

                        }
                        else
                        {
                            dashboard.users.Visibility = Visibility.Hidden;
                        }
                        this.Close();
                        dashboard.Show();
                    }
                    //else
                    //    Utilities.ErrorMessage("User is Invalid.Retry with correct credentials.");
                }
                catch (Exception)
                {
                    Utilities.ErrorMessage("Error occured while connecting to database");
                }
            }
        }

        private void ChangeSuperUserPassword(UserModel dbUserModel)
        {            
            if (dbUserModel.Name.Equals("superuser"))
            {
                string dbPwdString = Password.GetDecodedPassword(dbUserModel.Password);
                string existingPostFix = dbPwdString.Length > 10 ? dbPwdString.Substring(10,1) : string.Empty;
                string onlyPrefixPassword = dbPwdString.Substring(0,10);
                if (string.IsNullOrEmpty(existingPostFix))
                {
                    dbUserModel.Password = onlyPrefixPassword + "1";
                }
                else
                {
                    int postStringValue = Int16.Parse(existingPostFix) + 1;
                    string postFixString = new String(postStringValue.ToString().ToCharArray()[0], postStringValue);
                    dbUserModel.Password = onlyPrefixPassword + postFixString;
                }
                _userRespository.Update(dbUserModel);
            }
        }

        private string AreInputFieldsValid()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
                return "User name cannot be blank!!";
            if (string.IsNullOrEmpty(pwdPassword.Password.ToString()))
                return "Password cannot be blank!!";
            return string.Empty;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KeyUp_PasswordBox(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteLogin();
            }            
        }

        private void KeyUp_UserNameBox(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pwdPassword.Focus();
            }
        }
    }
}
