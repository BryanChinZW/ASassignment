<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reg2.aspx.cs" Inherits="ASassignment.Reg2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
    <script type="text/javascript">
        function Validate() {
            var str = document.getElementById('<%=Pass.ClientID %>').value;

            if (str.length < 8) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password length must be at least 8 characters";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return "Too Short"

            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password must contain at least 1 number";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return "No number"

            }
            else if (str.search(/[$&+,:;=?@#|'<>.^*()%!-]/) == -1) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password must contain at least 1 special character";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return "No special character";

            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password must contain at least 1 uppercase letter";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return "No uppercase"

            }

            else if (str.search(/[a-z]/) == -1) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password must contain at least 1 lowercase letter";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return "No lowercase"

            }
            document.getElementById('lbl_pwdchecker').innerHTML = "Excellent!";
            document.getElementById('lbl_pwdchecker').style.color = "Blue";
        }
        function Passcheck2() {
            var pass = document.getElementById('<%=Pass.ClientID %>').value;
            var pass2 = document.getElementById('<%=Pass2.ClientID %>').value;
            if (pass != pass2) {
                document.getElementById('lbl_password2checker').innerHTML = "Password not the same"
                document.getElementById('lbl_password2checker').style.color = "Red";
                return "Not Same"
            }
            document.getElementById('lbl_password2checker').innerHTML = "Excellent!";
            document.getElementById('lbl_password2checker').style.color = "Blue";
        }
    </script>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfTx0AaAAAAAEG2Nh9jt8JPpq2VGo6zOFxebi8M"></script>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Registration"></asp:Label>
            <br />
            <br />
            First Name
            <asp:TextBox ID="FName" runat="server"></asp:TextBox>
            <br />
            Last Name&nbsp;
            <asp:TextBox ID="LName" runat="server"></asp:TextBox>
            <br />
            Email&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="Email" runat="server"></asp:TextBox>
            <br />
            <br />
            Credit Card Info<br />
            Name on Card&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="CreditName" runat="server"></asp:TextBox>
            <br />
            Credit Card No&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="CreditNo" runat="server"></asp:TextBox>
            <br />
            Experation Date&nbsp;&nbsp;
            <asp:TextBox ID="CreditDate" runat="server" type="month"></asp:TextBox>
            <br />
            CVV
            <asp:TextBox ID="CVV" runat="server"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="passwordLabel" runat="server" Text="Password: "></asp:Label>
            <asp:TextBox ID="Pass" runat="server" TextMode="Password" onkeyup="javascript:Validate()"></asp:TextBox>
            <!---onkeyup="javascript:Validate()" --->
            <asp:Label ID="lbl_pwdchecker" runat="server"></asp:Label>
            <br />
            Confirm:&nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="Pass2" runat="server" TextMode="Password" onkeyup="javascript:Passcheck2()"></asp:TextBox>
            <asp:Label ID="lbl_password2checker" runat="server"></asp:Label>
            <br />
            <br />
            Date of Birth
            <asp:TextBox ID="DoB" runat="server" type="date" ></asp:TextBox>
            <br />
            <asp:Label ID="errorMsg" runat="server"></asp:Label>
            <br />
        </div>
        <asp:Button ID="submitBtn" runat="server" OnClick="submitBtn_Click" Text="Submit" />
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

