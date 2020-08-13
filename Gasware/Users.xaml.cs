using Gasware.Models;
using Gasware.Repository;
using Gasware.Repository.Interfaces;
using System.Windows;
using Unity;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        private string _username;
        private readonly IUnityContainer _container;
        private readonly IUserRespository _userRespository;

        public Users()
        {
            InitializeComponent();
        }

        public Users(IUnityContainer container, string username)
        {
            InitializeComponent();
            _username = username;
            _container = container;
            _userRespository = _container.Resolve<IUserRespository>();
        }

        private void btnNewInsertUser_Click(object sender, RoutedEventArgs e)
        {
            UserInsert userInsert = new UserInsert(_container, _username);
            userInsert.ShowDialog();
           // UserRepository userRepository = new UserRepository(_username);
            usersDataGrid.ItemsSource = _userRespository.GetUsers();
        }


        private void btnUserUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            UserModel userModel = (usersDataGrid.SelectedItem as UserModel);
            UserUpdate userUpdate = new UserUpdate(_container, userModel, _username);
            userUpdate.Title = "Updating User :" + userModel.UserId;
            userUpdate.AssignInterfaceValues();
            userUpdate.ShowDialog();
            //UserRepository userRepository = new UserRepository(_username);
            usersDataGrid.ItemsSource = _userRespository.GetUsers();
        }

        private void btnUserDelete_Clicked(object sender, RoutedEventArgs e)
        {
            UserModel userModel = (usersDataGrid.SelectedItem as UserModel);
            var result = MessageBox.Show("Are you sure you want to delete the User with id " +
              userModel.UserId + "?",
               "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.ToString() == "Yes")
            {
               // UserRepository userRepository = new UserRepository(_username);
                _userRespository.Delete(userModel);
                this.usersDataGrid.ItemsSource = _userRespository.GetUsers();
                MessageBox.Show("User deleted successfully.", "Success",
                   MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
      

        private void btnNewCloseUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCloseInsertUser_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
