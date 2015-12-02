using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;


namespace SendToast
{
    public partial class SendToast : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonSendToast_Click(object sender, EventArgs e)
        {
            try
            {
                string subcriptionUri = TextBoxUri.Text.ToString();
                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subcriptionUri);
                sendNotificationRequest.Method = "POST";
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<wp:Notification xmlns:wp=\"WPNotification\">" +
                    "<wp:Toast>" +
                        "<wp:Title>" + TextBoxTitle.Text.ToString() + "</wp:Title>" +
                        "<wp:SubTitle>" + TextBoxSubTitle.Text.ToString() + "</wp:SubTitle>" +
                        "<wp:Param>/Pages/HomePage.xaml?json=0988707727&amp;name=Apolong</wp:Param>" +
                    "</wp:Toast>" +
                    "</wp:Notification>";
                byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "2");

                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                    requestStream.Close();
                }
                    HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                    string notificationStatus = response.Headers["X-NotificationStatus"];
                    string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                    string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
                    TextBoxResponse.Text = notificationStatus + "|" + deviceConnectionStatus + "|" + notificationChannelStatus;
                    
                }
                
                catch(Exception ex)
                {
                    TextBoxResponse.Text = "Exception caught sending update: " +  ex.ToString();
                }
            }
    }
}