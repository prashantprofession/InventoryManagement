using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using System;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for UserUpdate.xaml
    /// </summary>
    public partial class UserUpdate : Window
    {
        public UserUpdate()
        {
            InitializeComponent();
        }
        private string _username;
        private readonly IUnityContainer _container;
        private UserModel _userModel;
        private readonly IUserRespository _userRespository;
        public UserUpdate(IUnityContainer container, UserModel userModel, string username)
        {
            InitializeComponent();
            _userModel = userModel; 
            _username = username;
            _container = container;
            _userRespository = _container.Resolve<IUserRespository>();

        }

        public void AssignInterfaceValues()
        {
            txtUserName.Text = _userModel.Name;
            //txtPassword.Text = _userModel.Password;
        }

        private void UpdateModelWithData()
        {
            _userModel = new UserModel()
            {
                UserId = _userModel.UserId,
                Name = txtUserName.Text,
                Password = txtPassword.Password.ToString()                
            };
        }

        private void btnSaveEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("Passwords donot match. Retry", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Hand);
                    return;
                }
                else if (txtUserName.Text.ToUpper() == "SUPERUSER")
                {
                    Utilities.ErrorMessage("Superuser is a system keyword.User cannot be added");
                    return;
                }

                UpdateModelWithData();
                //UserRepository userRepository = new UserRepository(_username);
                _userRespository.Update(_userModel);
                MessageBox.Show("User saved successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Error occured while saving customer data.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCloseEditUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }
}
