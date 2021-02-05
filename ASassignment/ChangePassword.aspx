<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ASassignment.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            old password<asp:TextBox ID="oldPass" TextMode="Password" runat="server"></asp:TextBox>
            <br />
            <br />
            new password<asp:TextBox ID="newPass" TextMode="Password" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="errorMsg" runat="server"></asp:Label>
            <br />
            <asp:Button ID="changBtn" runat="server" OnClick="changBtn_Click" Text="Change Password" />
        </div>
    </form>
</body>
</html>
