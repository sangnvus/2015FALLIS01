﻿#pragma checksum "H:\Hoc Tap\FPT Univerdity\Do an tot nghiep 2015\Github\2015FALLIS01\Demo Code\Demo App 15092015v4\Demo App 15092015v4\Views\Maps.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A710DBFB993419591093C56B05F3C705"
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
using Microsoft.Phone.Controls.Maps;
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


namespace Demo_App_15092015v4 {
    
    
    public partial class Maps : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.Maps.Map googlemap;
        
        internal Microsoft.Phone.Controls.Maps.MapTileLayer street;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Demo%20App%2015092015v4;component/Views/Maps.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.googlemap = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("googlemap")));
            this.street = ((Microsoft.Phone.Controls.Maps.MapTileLayer)(this.FindName("street")));
        }
    }
}

