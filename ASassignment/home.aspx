<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="home.aspx.cs" Inherits="ASassignment.home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        function alertMessage() {
            alert("please change password");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lbl_CreditName" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:Label ID="lbl_CreditNo" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:Label ID="lbl_creditDate" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:Label ID="lbl_CVV" runat="server" Text="Label"></asp:Label>
            <br />
            <br />
            <asp:Button ID="passChange" runat="server" OnClick="passChange_Click" Text="Change Password" />
            <br />
            <br />
            <asp:Button ID="SignOut" runat="server" Text="Sign Out " OnClick="SignOut_Click" />
        </div>
    </form>
</body>
</html>
