using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FT_Rider.Classes;
using Newtonsoft.Json;

namespace FT_Rider.Pages
{
    public partial class DemoPage : PhoneApplicationPage
    {

        string json = @"{
    'status': '0000',
    'lmd': 0,
    'content': {
        'fName': 'Tung',
        'lName': 'Quang',
        'interCode': null,
        'img': '',
        'mobile': '01246839943',
        'hAdd': 'TL904, Hòa Lộc, Tam Bình',
        'lan': 'vi',
        'cntry': null,
        'email': 'rider1@gmail.com',
        'rid': '2',
        'name': null,
        'oAdd': 'FU Hòa Lạc, Thạch Thất',
        'hAddLat': 10.074024941783989,
        'hAddLng': 105.99978480488062,
        'oAddLat': 21.008709822855284,
        'oAddLng': 105.53801635742184,
        'pmt': [
            {
                'pid': null,
                'cNO': '4909127391297639',
                'cvv': '6287',
                'mon': '9',
                'year': '2024',
                'postal': null,
                'cate': 'ORG'
            }
        ],
        'olmd': 1447214543000,
        'nlmd': 0,
        'status': 'VR',
        'uid': 'rider1@gmail.com',
        'pw': null
    }
}";
        public DemoPage()
        {
            InitializeComponent();

            RiderLogin rdLogin = new RiderLogin();
            rdLogin = JsonConvert.DeserializeObject<RiderLogin>(json);
            txt1.Text = rdLogin.content.email.ToString();
            //txt1.Text = rdLogin.

        }




        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}