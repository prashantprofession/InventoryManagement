using Gasware.Database;
using Gasware.Repository;
using System.IO;
using System.Windows;

namespace Gasware
{
    /// <summary>
    /// Interaction logic for DatabaseBackup.xaml
    /// </summary>
    public partial class DatabaseBackup : Window
    {
        public DatabaseBackup()
        {
            InitializeComponent();
            CompanyRepository companyRepository = new CompanyRepository("admin");
            txtFolderPath.Text = companyRepository.Get(1).ReportsPath;
        }

        private void RestoreDatabaseClicked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(txtFolderPath.Text))
            {
                if (txtFolderPath.Text.Substring(txtFolderPath.Text.Length - 4) != ".zip")
                {
                    Utilities.ErrorMessage("Not a zip file. Only zip files are allowed");
                    return;
                }
                DatabaseBackupProvider databaseBackupProvider = new DatabaseBackupProvider();
                databaseBackupProvider.RestoreDatabase(txtFolderPath.Text);
            }
            else
            {                
                Utilities.ErrorMessage("File doesn't exist");
            }
        }

        private void BackupDatabaseClicked(object sender, RoutedEventArgs e)
        {
            string path = txtFolderPath.Text;
            if (Directory.Exists(path))
            {
                var result = Utilities.YesNoMessage($@"Contents of folder {path} will be deleted.Do you still want to continue?");
                if (result == "No")
                    return;
                DatabaseBackupProvider databaseBackupProvider = new DatabaseBackupProvider();
                databaseBackupProvider.StartBackupProcess(path);
            }
            else
            {
                Utilities.ErrorMessage("Invalid directory specified.Back aborted!!!");
                return;
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
