﻿#pragma checksum "..\..\StockEntryInsert.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "054327F5C6118E66BAC3F209BBEF47F793EC6FA2ACE88B058F60B99586AD8346"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Gasware;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Gasware {
    
    
    /// <summary>
    /// StockEntryInsert
    /// </summary>
    public partial class StockEntryInsert : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label addStock;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox productCombo;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtUnitRate;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtQuantity;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBilledAmount;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dateReceived;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox receiversCombo;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtBalance;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtPaidAmount;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStockInSave;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\StockEntryInsert.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCloseAddStockIn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Gasware;component/stockentryinsert.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\StockEntryInsert.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.addStock = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.productCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\StockEntryInsert.xaml"
            this.productCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.productSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtUnitRate = ((System.Windows.Controls.TextBox)(target));
            
            #line 29 "..\..\StockEntryInsert.xaml"
            this.txtUnitRate.LostFocus += new System.Windows.RoutedEventHandler(this.CalculatteBillAmount);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtQuantity = ((System.Windows.Controls.TextBox)(target));
            
            #line 36 "..\..\StockEntryInsert.xaml"
            this.txtQuantity.LostFocus += new System.Windows.RoutedEventHandler(this.CalculatteBillAmount);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtBilledAmount = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.dateReceived = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 7:
            this.receiversCombo = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.txtBalance = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.txtPaidAmount = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.btnStockInSave = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\StockEntryInsert.xaml"
            this.btnStockInSave.Click += new System.Windows.RoutedEventHandler(this.btnSaveStockIn_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnCloseAddStockIn = ((System.Windows.Controls.Button)(target));
            
            #line 87 "..\..\StockEntryInsert.xaml"
            this.btnCloseAddStockIn.Click += new System.Windows.RoutedEventHandler(this.btnCloseAddStockIn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

