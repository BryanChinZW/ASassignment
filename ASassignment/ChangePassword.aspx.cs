using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASassignment
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            var passwordAge = Session["PasswordAge"];
            var passwordAgeTime = Convert.ToDateTime(passwordAge);
            var timenow = DateTime.Now;
            var result = DateTime.Compare(timenow, passwordAgeTime);
            if (result == -1)
            {
                Response.Redirect("Home.aspx");
            }
        }
        protected void changBtn_Click(object sender, EventArgs e)
        {
            var score = checkPassword(newPass.Text);
            if (score < 4)
            {
                errorMsg.Text = "Password not strong enough";
                errorMsg.ForeColor = Color.Red;
                return;
            }

            var email = Session["LoggedIn"];
            SHA512Managed hashing = new SHA512Managed();
            string gethash = "SELECT PasswordHash FROM ASusers WHERE Email = @Email";
            string getsalt = "SELECT PasswordSalt FROM ASusers WHERE Email = @Email";
            using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
            {
                connection.Open();
                SqlCommand hashcommand = new SqlCommand(gethash, connection);
                SqlCommand saltcommand = new SqlCommand(getsalt, connection);
                hashcommand.Parameters.AddWithValue("@Email", email);
                saltcommand.Parameters.AddWithValue("@Email", email);
                string hash = hashcommand.ExecuteScalar().ToString();
                string salt = saltcommand.ExecuteScalar().ToString();
                var pwdWithSalt = oldPass.Text + salt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                var finalHash = Convert.ToBase64String(hashWithSalt);
                if (hash == finalHash)
                {
                    string checkpass1 = "SELECT Password1 FROM [PasswordHistory] WHERE Email = @Email";
                    string checkpass2 = "SELECT Password2 FROM [PasswordHistory] WHERE Email = @Email";
                    SqlCommand checkpass1command = new SqlCommand(checkpass1, connection);
                    SqlCommand checkpass2command = new SqlCommand(checkpass2, connection);
                    checkpass1command.Parameters.AddWithValue("@Email", email);
                    checkpass2command.Parameters.AddWithValue("@Email", email);
                    string pass1 = checkpass1command.ExecuteScalar().ToString();
                    string pass2 = checkpass2command.ExecuteScalar().ToString();
                    var newpwdWithSalt = newPass.Text + salt;
                    byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpwdWithSalt));
                    var newfinalHash = Convert.ToBase64String(newhashWithSalt);
                    if(newfinalHash == pass1)
                    {
                        errorMsg.Text = "Please use another password";
                        errorMsg.ForeColor = Color.Red;
                        return;
                    }
                    if (newfinalHash == pass2)
                    {
                        errorMsg.Text = "Please use another password";
                        errorMsg.ForeColor = Color.Red;
                        return;
                    }
                    string updatePassword = "UPDATE [ASusers] SET PasswordHash = @PasswordHash WHERE Email = @Email";
                    SqlCommand updateCommand = new SqlCommand(updatePassword, connection);
                    updateCommand.Parameters.AddWithValue("@PasswordHash", newfinalHash);
                    updateCommand.Parameters.AddWithValue("@Email", email);
                    updateCommand.ExecuteNonQuery();
                    
                    using (SqlCommand cmd = new SqlCommand("UPDATE [PasswordHistory] SET Password1 = @PasswordHash1, Password2 = @PasswordHash2 ,TimeCreated = @TimeCreated WHERE Email = @Email",connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@PasswordHash1", newfinalHash);
                            cmd.Parameters.AddWithValue("@PasswordHash2", finalHash);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@TimeCreated", DateTime.Now.ToString());
                            
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
                else
                {
                    errorMsg.Text = "Password is incorrect";
                }
                connection.Close();
                Session["PasswordAge"] = DateTime.Now.AddMinutes(5).ToString();
                Response.Redirect("Home.aspx");
            }
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
    }
    
}