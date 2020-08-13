using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gasware.Database
{
    public static class Utilities
    {
        public static string GetDatabaseDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd H:mm:ss");
        }

        public static string GetDatabaseShortDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }


        public static string CurrentDBTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
        }

        public static string CurrentDbDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }


        public static void ErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void InformationMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void WarningMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static void SuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public static string YesNoMessage(string message)
        {
           var response = MessageBox.Show(message, "Question", MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
            return response.ToString();
        }

    }
}
