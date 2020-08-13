using Gasware.Common;
using Gasware.Repository;
using iTextSharp.text.pdf;
using Shell32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.util;

namespace Gasware.Database
{
    public class DatabaseBackupProvider
    {
        public void StartBackupProcess(string dirPath)
        {
            string fullFilepath = GetBackupSqlFileNameWithPath(dirPath);
            string mysqlArguments = GetBackupArguments();
            CleanUpFiles(dirPath);
            ExecuteCommand(dirPath, fullFilepath, mysqlArguments);            
            Utilities.InformationMessage("Database backup & zip completed file Successfully!");

        }

        private string GetMySqlDumpExePath()
        {
            string sqlForInstalledPath = "SELECT @@basedir;";
            return DbQueryExecuter.GetSingleStringForQuery(sqlForInstalledPath) + @"/bin/mysqldump.exe";
        }

        private string GetBackupDirPath()
        {
            CompanyRepository companyRepository = new CompanyRepository("admin");
            return companyRepository.Get(1).ReportsPath + @"\";
        }

        private string GetBackupArguments()
        {
            string directoryPath = GetBackupDirPath();
            string sqlBackupFileName = $@"{directoryPath}DBBackup_" +
                                       $@"{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.bkp.sql";
            string executeCommand = $@" --replace --skip-extended-insert -u{DatabaseConnection.MySqlUserId} " +
                                    $@"-p{DatabaseConnection.MySqlPassword} " +
                                    $@"-h {DatabaseConnection.MySqlServer} " +
                                    $@"{DatabaseConnection.MySqlDatabase}";
            return executeCommand;        
        }

        
        private string GetBackupSqlFileNameWithPath(string dirPath)
        {
            //string directoryPath = GetBackupDirPath();
            string path = $@"{dirPath}\\DBBackup_" +
                          $@"{DateTime.Now.ToString("yyyyMMddHHmmss")}.bkp.sql";
            return path;
        }

        private void ExecuteCommand(string directory, string path, string mysqlArguments)
        {
            string mysqlDumpExeFilePath = GetMySqlDumpExePath();
            StreamWriter file = new StreamWriter(path);
            ProcessStartInfo psi = new ProcessStartInfo();            
            psi.FileName = mysqlDumpExeFilePath;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.Arguments = mysqlArguments;
            psi.UseShellExecute = false;
            Process process = Process.Start(psi);
            string output;
            output = process.StandardOutput.ReadToEnd();
            file.WriteLine(output);         
            process.WaitForExit();            
            file.Close();
            process.Close();            
            string zipFileName = CompressDirectory(directory);
            var response = Utilities.YesNoMessage("Do you want to send backup to the E-Mail?");
            if (response == "No")
                return;
            SendBackupEmail(zipFileName);
        }

        private void CleanUpFiles(string absoluteDir)
        {
            string zipFileName = absoluteDir + ".zip";
            if (File.Exists(zipFileName))
                File.Delete(zipFileName);
            Array.ForEach(Directory.GetFiles(absoluteDir),
              delegate (string path) { File.Delete(path); });
        }

        private static string CompressDirectory(string absoluteDir)
        {
            string zipFileName = absoluteDir + ".zip";
            ZipFile.CreateFromDirectory(absoluteDir, zipFileName);
            return zipFileName;
        }

        private static void SendBackupEmail(string zipFileName)
        {
            EmailSendProvider emailSendProvider = new EmailSendProvider();
            emailSendProvider.SendBackupEmail(zipFileName);
        }

        public void RestoreDatabase(string absoluteFileName)
        {
            string folderName = absoluteFileName.Substring(0, absoluteFileName.Length - 4) + "restoredbbackup/";
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            ZipFile.ExtractToDirectory(absoluteFileName, folderName);

            var directory = new DirectoryInfo(folderName);
            var latestRestoredFile = directory.GetFiles()
                                        .OrderByDescending(f =>
                                        f.LastWriteTime).First();
            string restoreArgument = GetRestoreArguments(latestRestoredFile);
            ExecuteCommand(folderName, absoluteFileName, restoreArgument);
        }

        private string GetRestoreArguments(FileInfo filename)
        {
            //string directoryPath = GetBackupDirPath();
            string sqlBackupFileName = $@"{filename.Directory}/{filename.Name}";
            string executeCommand = $@" -u{DatabaseConnection.MySqlUserId} " +
                                    $@"-p{DatabaseConnection.MySqlPassword} " +
                                    $@"-h {DatabaseConnection.MySqlServer} " +
                                    $@"{DatabaseConnection.MySqlDatabase}<{sqlBackupFileName}";
            return executeCommand;
        }


    }
}
