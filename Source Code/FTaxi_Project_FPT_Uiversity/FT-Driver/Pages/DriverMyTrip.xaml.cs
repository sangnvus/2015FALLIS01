using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Diagnostics;
using FT_Driver.Classes;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;

namespace FT_Driver.Pages
{
    public partial class DriverMyTrip : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData;
        string password = string.Empty;
        string userId = string.Empty;
        public DriverMyTrip()
        {
            InitializeComponent();
            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new DriverLogin();
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                password = (string)tNetUserLoginData["PasswordMd5"];
                userId = (string)tNetUserLoginData["UserId"];
            }

            //Load My trip
        }

        private void ShowDriverLostAssets()
        {
            (this.Resources["showDriverLostAssets"] as Storyboard).Begin();
            grv_DriverAssetsLost.Visibility = Visibility.Visible;
        }
        private void HideDriverLostAssets()
        {
            grv_DriverAssetsLost.Visibility = Visibility.Collapsed;
        }
        private void btn_ViewTripDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_AlertAssets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Content.Text.Equals(string.Empty))
            {
                MessageBox.Show("Vui lòng cung cấp thêm một vài thông tin. Xin cảm ơn.");
                txt_Content.Focus();
            }
            else
            {
                SendFeedback();
            }
        }

        private async void SendFeedback()
        {
            ShowLoadingScreen();
            var did = userData.content.driverInfo.did;
            var title = userData.content.driverInfo.fName + " " + userData.content.driverInfo.lName + " Báo mất đồ";
            var content = txt_Content.Text;
            var uid = userId;
            var pw = password;
            var input = string.Format("{{\"did\":\"{0}\",\"title\":\"{1}\",\"content\":\"{2}\",\"uid\":\"{3}\",\"pw\":\"{4}\"}}", did, title, content, uid, pw);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverSendFeedback, input);
                var lostStatus = JsonConvert.DeserializeObject<BaseResponse>(output);

                if (lostStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu OK thi
                    HideLoadingScreen();
                    HideDriverLostAssets();
                    MessageBox.Show("Thông báo của bạn đã được gửi lên thành công."); //Thêm lái xe thành công

                }
                else
                {

                    Debug.WriteLine("Có lỗi 265gyut ở driver send lost");
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                MessageBox.Show("(Mã lỗi 5689) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi ghjtr54 ở Lost Asset");
            }
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void btn_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_AddFavorite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_Close_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void lls_MyTrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void img_CloseDriverLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideDriverLostAssets();
        }

        private void btn_SendLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowDriverLostAssets();
        }
    }
}