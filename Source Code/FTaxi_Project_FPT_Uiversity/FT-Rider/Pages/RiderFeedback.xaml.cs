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
using FT_Rider.Classes;
using Newtonsoft.Json;

namespace FT_Rider.Pages
{
    public partial class RiderFeedback : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        string pwmd5 = string.Empty;

        public RiderFeedback()
        {
            InitializeComponent();

            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
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
            var uid = userData.content.uid;
            var pw = pwmd5;
            var rid = userData.content.rid;
            var title = txt_Subject.Text;
            var content = txt_Content.Text;

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"rid\":\"{2}\",\"title\":\"{3}\",\"content\":\"{4}\"}}", uid, pw, rid, title, content);

            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderSendFeedback, input);
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
                MessageBox.Show("(Mã lỗi 11520) " + ConstantVariable.errConnectingError);
            }
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }

        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }


        private void ShowMessageERR_SYSTEM()
        {
            MessageBox.Show("(Mã lỗi 11521) " + ConstantVariable.ERR_SYSTEM);
            this.Focus();
        }


    }
}