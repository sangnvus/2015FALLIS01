﻿#pragma checksum "H:\Lap trinh Windows phone\Demo code\Demo App 15092015v4\Demo App 15092015v4\Views\LoginPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DBA13EE8D38A9534FB4131E27CBA2A01"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace LoginApp.Views {
    
    
    public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox UserName;
        
        internal System.Windows.Controls.PasswordBox PassWord;
        
        internal System.Windows.Controls.Button Login;
        
        internal System.Windows.Controls.Button SignUp;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Demo%20App%2015092015v4;component/Views/LoginPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.UserName = ((System.Windows.Controls.TextBox)(this.FindName("UserName")));
            this.PassWord = ((System.Windows.Controls.PasswordBox)(this.FindName("PassWord")));
            this.Login = ((System.Windows.Controls.Button)(this.FindName("Login")));
            this.SignUp = ((System.Windows.Controls.Button)(this.FindName("SignUp")));
        }
    }
}

