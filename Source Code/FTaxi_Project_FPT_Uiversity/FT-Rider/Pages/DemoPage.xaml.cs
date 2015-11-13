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

        string nearCar = @"{
    'status': '0000',
    'lmd': 0,
    'content': {
        'listDriverDTO': [
            {
                'did': '85f9049d-7209-4173-8aa3-bf354a91a54d',
                'fName': 'Đạo Dino in Storm',
                'lName': 'Nguyễn Minh',
                'cName': 'Hoàng Hà Taxi',
                'mobile': '+84 16463668688',
                'rate': null,
                'oPrice': 11000,
                'oKm': 0.6,
                'f1Price': 13000,
                'f1Km': 20,
                'f2Price': 11000,
                'f2Km': 0,
                'f3Price': null,
                'f3Km': null,
                'f4Price': null,
                'f4Km': null,
                'img': '',
                'lat': 21.075741,
                'lng': 105.78757
            },
            {
                'did': '4',
                'fName': 'Nguyễnn',
                'lName': 'Hoài Nam',
                'cName': 'Hoàng Hà Taxi',
                'mobile': '09777556231',
                'rate': null,
                'oPrice': 9000,
                'oKm': 0.8,
                'f1Price': 11000,
                'f1Km': 15,
                'f2Price': 10500,
                'f2Km': 15,
                'f3Price': null,
                'f3Km': null,
                'f4Price': null,
                'f4Km': null,
                'img': 'D:\\TNET\\.metadata\\.plugins\\org.eclipse.wst.server.core\\tmp0\\wtpwebapps\\TaxiNet\\data\\uploaded\\driver\\image\\VN\\null\\2.jpg',
                'lat': 21.071871,
                'lng': 105.783251
            }
        ]
    }
}";
        
        public DemoPage()
        {
            InitializeComponent();

            RiderLogin rdLogin = new RiderLogin();
            rdLogin = JsonConvert.DeserializeObject<RiderLogin>(json);
            //txt1.Text = rdLogin.content.email.ToString();
            //txt1.Text = rdLogin.

            RiderGetNearDriver getNearCar = new RiderGetNearDriver();
            getNearCar = JsonConvert.DeserializeObject<RiderGetNearDriver>(nearCar);
            txt1.Text = getNearCar.content.listDriverDTO[0].fName.ToString();
            

        }




        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}