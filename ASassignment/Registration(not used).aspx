 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration(not used).aspx.cs" Inherits="ASassignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript">
        function Passcheck() {
            var pass = document.getElementById('<%=Pass.ClientID%>').value
            if (pass.length < 8) {
                document.getElementById('PassCheck').innerHTML = "Password length must be least  8 Characters"
                document.getElementById('PassCheck').style.colour = "Red";
                return ("too short");
            } else if (pass.search(/[0-9]/ == -1){
                document.getElementById('PassCheck').innerHTML = "Passsword must contain numbers"
                document.getElementById('PassCheck').style.colour = "Red";
                return ("no number");
            } else if (pass.search(/[a-z]/ == -1){
                document.getElementById('PassCheck').innerHTML = "Password must contain lowercase letters"
                document.getElementById('PassCheck').style.colour = "Red";
                return ("no lower");
            } else if (pass.search(/[A-Z]/ == -1){
                document.getElementById('PassCheck').innerHTML = "Password must contain uppercase letters"
                document.getElementById('PassCheck').style.colour = "Red";
                return ("no upper");
            } else if (pass.search(/[^0-9a-zA-Z]/) == -1) {
                document.getElementById('PassCheck').innerHTML = "Password must contain special characters"
                document.getElementById('PassCheck').style.colour = "Red";
                return ("no special");
            } else {
                document.getElementById('PassCheck').innerHTML = "Strong";
                document.getElementById('PassCheck>').style.color = "Green";

            }
        }
        function Passcheck2() {
            var pass = document.getElementById('PassCheck2').value
            var pass2 = document.getElementById('PassCheck2').value
            if (pass != pass2) {
                document.getElementById('PassCheck2').innerHTML = "Password not the same"
                document.getElementById('PassCheck2').style.color = "Red";
            } else {
                document.getElementById('PassCheck2').innerHTML = "";
            }
        }
       
    </script>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style3 {
            width: 180px;
        }
        .auto-style6 {
            width: 180px;
            height: 28px;
        }
        .auto-style7 {
            height: 28px;
        }
        .auto-style9 {
            width: 154px;
            height: 28px;
        }
        .auto-style10 {
            width: 154px;
        }
        .auto-style11 {
            width: 118px;
            height: 28px;
        }
        .auto-style12 {
            width: 118px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style9">First Name</td>
                    <td class="auto-style6">
                        <asp:TextBox ID="FName" runat="server"></asp:TextBox>
                    </td>
                    <td class="auto-style11">Last Name </td>
                    <td class="auto-style7">
                        <asp:TextBox ID="LName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style9">Email</td>
                    <td class="auto-style6">
                        <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                    </td>
                    <td class="auto-style11">&nbsp;</td>
                    <td class="auto-style7">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">-------------Credit Card info-------------</td>
                </tr>
                <tr>
                    <td class="auto-style10">Name on Card</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="CreditName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">Credit Card No.</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="CreditNo" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">Experation date</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="CreditDate" runat="server"></asp:TextBox>
                        &nbsp; </td>
                    <td class="auto-style12">CVV</td>
                    <td>
                        <asp:TextBox ID="CVV" runat="server" Width="73px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style10">Password</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="Pass" runat="server" TextMode="Password" onkeyup="javascript:Passcheck()" ></asp:TextBox>
                    </td>
                    <td class="auto-style12">
                            <asp:Label ID="PassCheck" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">Confirm Password</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="Pass2" runat="server" TextMode="Password" onkeyup="javascript:Passcheck2()"></asp:TextBox>
                    </td>
                    <td class="auto-style12">
                            <asp:Label ID="PassCheck2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">
                        &nbsp;</td>
                    <td class="auto-style12">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">&nbsp;</td>

                </tr>
                <tr>
                    <td class="auto-style10">Date of Brith</td>
                    <td class="auto-style3">
                        <asp:TextBox ID="DoB" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">
                        <asp:Label ID="errorMsg" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style10">&nbsp;</td>
                    <td class="auto-style3">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td class="auto-style10">
                        <asp:Button ID="submitBtn" runat="server" Text="Sign up" OnClick ="submitBtn_Click"/>
                    </td>
                    <td class="auto-style3">
                        &nbsp;</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
