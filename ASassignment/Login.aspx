<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ASassignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 136px;
        }
        .auto-style3 {
            width: 136px;
            height: 26px;
        }
        .auto-style4 {
            height: 26px;
        }
    </style>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfTx0AaAAAAAEG2Nh9jt8JPpq2VGo6zOFxebi8M"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">Email</td>
                    <td>
                        <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">Password</td>
                    <td>
                        <asp:TextBox ID="Password" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">&nbsp;</td>
                    <td>
                        <asp:Label ID="errorMsg" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style2">
                        <asp:Button ID="loginBtn" runat="server" OnClick="loginBtn_Click" Text="Login" />
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </div>
        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute("6LfTx0AaAAAAAEG2Nh9jt8JPpq2VGo6zOFxebi8M", { action: 'Submit' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
