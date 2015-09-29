<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendToast.aspx.cs" Inherits="SendToast.SendToast" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin-left: 40px">
    
        <asp:Label ID="Label1" runat="server" Text="Enter URL"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBoxUri" runat="server"></asp:TextBox>
    
    </div>
        <p style="margin-left: 40px">
            <asp:Label ID="Label2" runat="server" Text="Enter Title"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TextBoxTitle" runat="server"></asp:TextBox>
        </p>
        <p style="margin-left: 40px">
            <asp:Label ID="Label3" runat="server" Text="Enter SubTitle"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TextBoxSubTitle" runat="server"></asp:TextBox>
        </p>
        
        <p style="margin-left: 40px">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="ButtonSendToast" runat="server" OnClick="ButtonSendToast_Click" Text="Send Toast Notification" />
        </p>
        <p style="margin-left: 40px">
            <asp:Label ID="Label4" runat="server" Text="Response Result"></asp:Label>
&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TextBoxResponse" runat="server"></asp:TextBox>
        </p>
    </form>
</body>
</html>
