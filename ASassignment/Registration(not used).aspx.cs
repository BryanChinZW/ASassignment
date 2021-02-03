using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;

namespace ASassignment
{
    public partial class Registration : System.Web.UI.Page
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

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-s]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^0-9a-zA-Z]"))
            {
                score++;
            }
            return score;
        }
        
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            var fname = HttpUtility.HtmlEncode(FName.Text);
            var lname = HttpUtility.HtmlEncode(LName.Text);
            var email = HttpUtility.HtmlEncode(Email.Text);
            var creditname = HttpUtility.HtmlEncode(CreditName.Text);
            var creditno = HttpUtility.HtmlEncode(CreditNo.Text);
            var creditdate = HttpUtility.HtmlEncode(CreditDate.Text);
            var cvv = HttpUtility.HtmlEncode(CVV.Text);
            var pass = HttpUtility.HtmlEncode(Pass.Text);
            var pass2 = HttpUtility.HtmlEncode(Pass2.Text);
            var dob = HttpUtility.HtmlEncode(DoB.Text);
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
            if (Regex.IsMatch(creditno, @"\d{16}"))
            {
                error = error + " Enter credit number  </br>";
            }
            if (Regex.IsMatch(creditdate, "^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$"))
            {
                error = error + " Enter credit date  </br>";
            }
            if (Regex.IsMatch(cvv, @"\d{3}"))
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
            if (Regex.IsMatch(dob, "^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$"))
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
            int scores = checkPassword(Pass.Text);
            if (scores < 4)
            {
                return;
            }
            using (var con = new SqlConnection(MYDBConnectionString))
            {
                con.Open();
                var check = "SELECT * FROM [ASusers] WHERE Email = @Email";
                var insert = "INSERT INTO [ASusers] VALUES(@Email,@Name, @PasswordHash, @PasswordSalt, @CreditName, @CreditNo, @CreditDate, @CVV, @DoB, @IV, @Key)";
                var insertpass = "INSERT INTO [PasswordHistory] VALUES(@PasswordHash1, @PasswordHash2, @Email, @TimeCreated)";
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
                    cmd.Parameters.AddWithValue("@CreditName", encryptData(CreditName.Text.Trim()));
                    cmd.Parameters.AddWithValue("@CreditNo", encryptData(CreditNo.Text.Trim()));
                    cmd.Parameters.AddWithValue("@CreditDate", encryptData(CreditDate.Text.Trim()));
                    cmd.Parameters.AddWithValue("@CVV", encryptData(CVV.Text.Trim()));
                    cmd.Parameters.AddWithValue("@DoB", DoB.Text.Trim());
                    cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                    cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("user created");
                }
                using (SqlCommand cmd = new SqlCommand(insertpass, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@PasswordHash1", finalHash);
                    cmd.Parameters.AddWithValue("@PasswordHash2", "placeholder");
                    cmd.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    cmd.Parameters.AddWithValue("@TimeCreated", DateTime.Now.ToString());
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                Response.Redirect("Login.aspx");

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

    }
}