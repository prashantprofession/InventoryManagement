using Gasware.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Gasware.Common
{
    public class PrintFunction
    {

        public void PrintList<T>(IEnumerable<T> items)
        {            
            PrintDialog myPrintDialog = new PrintDialog();
            if (myPrintDialog.ShowDialog() == true)
            {
                myPrintDialog.PrintVisual((Visual)items, "Listbox Items Print");
            }
        }

        public void PrintGrid(DataGrid dataGrid)
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintVisual(dataGrid, "Grid Printing.");
                Utilities.SuccessMessage("Printed successfully");
            }
        }        
    }
}
