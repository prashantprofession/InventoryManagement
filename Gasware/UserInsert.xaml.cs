using Gasware.Database;
using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System.Runtime.CompilerServices;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for UserInsert.xaml
    /// </summary>
    public partial class UserInsert : Window
    {
        public UserInsert()
        {
            InitializeComponent();
        }

        private string _username;
        private readonly IUnityContainer _container;
        private readonly IUserRespository _userRespository;

        public UserInsert(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _userRespository = _container.Resolve<IUserRespository>();
        }

      

        private void ClearFields()
        {
            txtUserName.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtConfirmPassword.Password = string.Empty;
        }

        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Passwords donot match. Re-try.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Hand);
                return;
            }
            else if (txtUserName.Text.ToUpper()=="SUPERUSER")
            {
                Utilities.ErrorMessage("Superuser is a system keyword.User cannot be added");
                return;
            }

            UserModel userModel = new UserModel()
            {
                Name = txtUserName.Text,
                Password = txtPassword.Password.ToString()
            };

            // UserRepository userRepository = new UserRepository(_username);
            _userRespository.Create(userModel);
            MessageBox.Show("User added successfully ", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFields();
        }

        private void btnCloseAddUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
       
    }
}
