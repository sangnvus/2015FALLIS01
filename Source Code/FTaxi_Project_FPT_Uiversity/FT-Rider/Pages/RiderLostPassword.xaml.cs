using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FT_Rider.Pages
{
    public partial class RiderLostPassword : PhoneApplicationPage
    {
        public RiderLostPassword()
        {
            InitializeComponent();
        }

        private void txtEmail_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Email.Text = String.Empty;
        }
    }
}