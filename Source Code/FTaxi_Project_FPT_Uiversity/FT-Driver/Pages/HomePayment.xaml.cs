using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using FT_Driver.Classes;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FT_Driver.Pages
{
    public partial class HomePayment : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetTripData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverCompleteTrip completeTrip;
        string myPassword;
        
        
        public HomePayment()
        {
            InitializeComponent();
            if (tNetTripData.Contains("CompleteTripBill"))
            {
                completeTrip = new DriverCompleteTrip();
                completeTrip = (DriverCompleteTrip)tNetTripData["CompleteTripBill"];
                myPassword = (string)tNetUserLoginData["PasswordMd5"];
            }
        }

        

        private async void btn_Payment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Kiểm tra mật khẩu
            //MD5.MD5 pw = new MD5.MD5();
            //pw.Value = txt_Password.ActionButtonCommandParameter.ToString();
            string pw = myPassword;
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"tid\":\"{2}\",\"eAdd\":\"{3}\",\"eCityName\":\"{4}\",\"eLat\":\"{5}\",\"eLng\":\"{6}\",\"dis\":\"{7}\",\"fare\":\"{8}\",\"lmd\":\"{9}\"}}", completeTrip.uid, pw, completeTrip.tid, completeTrip.eAdd, completeTrip.eCityName, completeTrip.eLat, completeTrip.eLng, completeTrip.dis, completeTrip.fare, completeTrip.lmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverCompleteTrip, input);
                if (output != null)
                {
                    var completeStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (completeStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))
                    {
                        ///1. HIện thông báo thành ôcng
                        ///2. xóa toàn bộ thôn tin trip
                        ///3. Về màn hình Home

                        //1
                        MessageBox.Show("Thanh toán thành công. Chúc bạn ngày làm việc hiệu quả!");

                        //2.
                        tNetTripData.Remove("CompleteTripBill");

                        //3.
                        NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 901) " + ConstantVariable.errServerError);
                Debug.WriteLine("Mã lỗi 15fht không lấy get json string từ completetrip");
            }
            //Xóa dữ liệu
        }
    }
}