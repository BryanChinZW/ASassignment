using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ASassignment.Login;

namespace ASassignment
{
    public partial class Reg2 : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private int checkPassword(string password)
        {
            int score = 0;

            //if password length is less than 8
            if (password.Length < 8)
            {
                return 1;
            }
            //score 1
            else
            {
                score = 1;
            }
            //score 2
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            //score 3
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            //score 4
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[$&+,:;=?@#|'<>.^*()%!-]"))
            {
                score++;
            }

            return score;
        }

        protected void submitBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                // implement codes for the button event
                // Extract data from textbox
                int scores = checkPassword(Pass.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            lbl_password2checker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_password2checker.ForeColor = Color.Red;
                return;
            }
            lbl_password2checker.ForeColor = Color.Green;
            var fname = FName.Text.ToString();
            var lname = LName.Text.ToString();
            var email = Email.Text.ToString();
            var creditname = CreditName.Text.ToString();
            var creditno = CreditNo.Text.ToString();
            var creditdate = CreditDate.Text.ToString();
            var cvv = CVV.Text.ToString();
            var pass = Pass.Text.ToString();
            var pass2 = Pass2.Text.ToString();
            var dob = DoB.Text.ToString();
            var error = "";
            if (fname == "")
            {
                error = error + " Enter First Name </br>";
            }
            if (lname == "")
            {
                error = error + "  Enter Last Name  </br>";
            }
            var validEmail = IsValidEmail(email);
            if (!validEmail)
            {
                error = error + " Enter Email  </br>";
            }
            if (creditname == "")
            {
                error = error + " Enter credit name  </br>";
            }
            if (!Regex.IsMatch(creditno, @"\d{16}"))
            {
                error = error + " Enter credit number  </br>";
            }
            if (creditdate == "")//CHANGE
            {
                error = error + " Enter credit date  </br>";
            }
            if (!Regex.IsMatch(cvv, @"\d{3}"))
            {
                error = error + " Enter cvv  </br>";
            }
            if (pass == "")
            {
                error = error + " Enter password  </br>";
            }
            if (pass2 == "")
            {
                error = error + " Enter Confirm password  </br>";
            }
            if (dob == "")
            {
                error = error + " Enter date of birth  </br>";
            }
            if (error != "")
            {
                errorMsg.Text = error;
                errorMsg.ForeColor = Color.Red;
                errorMsg.Visible = true;
                Debug.WriteLine(error);
                return;
            }
            using (var con = new SqlConnection(MYDBConnectionString))
            {
                con.Open();
                var check = "SELECT * FROM [ASusers] WHERE Email = @Email";
                var insert = "INSERT INTO [ASusers] VALUES(@Email,@Name, @PasswordHash, @PasswordSalt, @CreditName, @CreditNo, @CreditDate, @CVV, @DoB, @IV, @Key, @IsTimeout, @Timeout)";
                var insertpass = "INSERT INTO [PasswordHistory] VALUES(@Email,@PasswordHash1, @PasswordHash2, @TimeCreated)";
                using (SqlCommand cmd = new SqlCommand(check, con))
                {
                    cmd.Parameters.AddWithValue("@Email", Email.Text);
                    bool exist = Convert.ToBoolean(cmd.ExecuteScalar());
                    if (exist)
                    {
                        errorMsg.Text = "Email already exist";
                        errorMsg.ForeColor = Color.Red;
                        errorMsg.Visible = true;
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    string pwd = Pass.Text.ToString();

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();


                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    var name = FName.Text + LName.Text;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                    cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    cmd.Parameters.AddWithValue("@CreditName", Convert.ToBase64String(encryptData(CreditName.Text.Trim())));
                    cmd.Parameters.AddWithValue("@CreditNo", Convert.ToBase64String(encryptData(CreditNo.Text.Trim())));
                    cmd.Parameters.AddWithValue("@CreditDate", Convert.ToBase64String(encryptData(CreditDate.Text.Trim())));
                    cmd.Parameters.AddWithValue("@CVV", Convert.ToBase64String(encryptData(CVV.Text.Trim())));
                    cmd.Parameters.AddWithValue("@DoB", DoB.Text.Trim());
                    cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                    cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                    cmd.Parameters.AddWithValue("@IsTimeout", 0);
                    cmd.Parameters.AddWithValue("@Timeout", DateTime.Now.ToString());
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("user created");
                }
                using (SqlCommand cmd = new SqlCommand(insertpass, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    cmd.Parameters.AddWithValue("@PasswordHash1", finalHash);
                    cmd.Parameters.AddWithValue("@PasswordHash2", "placeholder");
                    cmd.Parameters.AddWithValue("@TimeCreated", DateTime.Now.ToString());
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                Response.Redirect("Login.aspx");

            }
        }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            ("https://google.com/recaptcha/api/siteverify?secret=6LfTx0AaAAAAAIi5HZSN-bKZQescUmjntB_B0tub &response=" + captchaResponse);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }

        }


    }
}