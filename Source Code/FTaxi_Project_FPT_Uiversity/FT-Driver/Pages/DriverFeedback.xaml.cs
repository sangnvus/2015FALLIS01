using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FT_Driver.Classes;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;

namespace FT_Driver.Pages
{
    public partial class DriverFeedback : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData = null;
        string pwmd5 = string.Empty;
        string userId = string.Empty;

        public DriverFeedback()
        {
            InitializeComponent();
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new DriverLogin();
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                userId = (string)tNetUserLoginData["UserId"];
            }
        }

        private void btn_Send_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Content.Text.Equals(string.Empty) && txt_Subject.Text.Equals(string.Empty))
            {
                MessageBox.Show("Vui lòng điền đẩy đủ thông tin theo yêu cầu.");
            }
            else
            {
                SendFeedback();
            }
        }

        private async void SendFeedback()
        {
            ShowLoadingScreen();
            var uid = userId;
            var pw = pwmd5;
            var did = userData.content.driverInfo.did;
            var title = txt_Subject.Text;
            var content = txt_Content.Text;

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"rid\":\"{2}\",\"title\":\"{3}\",\"content\":\"{4}\"}}", uid, pw, did, title, content);

            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverSendFeedback, input);
                var sendStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                switch (sendStatus.status)
                {
                    case ConstantVariable.RESPONSECODE_SUCCESS:
                        HideLoadingScreen();
                        MessageBox.Show(ConstantVariable.strSendFeedbackOK);
                        break;
                    case ConstantVariable.RESPONSECODE_ERR_SYSTEM:
                        HideLoadingScreen();
                        ShowMessageERR_SYSTEM();
                        break;
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                MessageBox.Show("(Mã lỗi 23698) " + ConstantVariable.errConnectingError);
            }
        }

        private void ShowMessageERR_SYSTEM()
        {
            MessageBox.Show("(Mã lỗi 2565) " + ConstantVariable.ERR_SYSTEM);
            this.Focus();
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }

        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }
    }
}