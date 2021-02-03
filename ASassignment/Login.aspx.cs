using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ASassignment
{
    public partial class Login : System.Web.UI.Page
    {
        string timeout;
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string salt;
        string hash;
        string id;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[ "Attempts"] == null)
            {
                Session["Attempts"] = 0;
            }
            if (Session["Timout"] == null)
            {
                Session["Timeout"] = DateTime.Now.ToString();
            }
            

        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            var sessiontimeout = Session["Timeout"];
            var sessiontimeoutDate = Convert.ToDateTime(sessiontimeout);
            var sessiontimenow = DateTime.Now;
            var sessionresult = DateTime.Compare(sessiontimenow, sessiontimeoutDate);
            if (sessionresult == -1)
            {
                errorMsg.Text = "please try agian later";
                errorMsg.ForeColor = Color.Red;
                return;
            }
            var email = HttpUtility.HtmlEncode(Email.Text);
            var password = HttpUtility.HtmlEncode(Password.Text);
            if (email == "")
            {
                errorMsg.Text = "please fill in all values";
                errorMsg.ForeColor = Color.Red;
                return;
            }
            if (password == "")
            {
                errorMsg.Text = "please fill in all values";
                errorMsg.ForeColor = Color.Red;
                return;
            }
            if (ValidateCaptcha())
            {
                using (var con = new SqlConnection(MYDBConnectionString))
                {
                    con.Open();
                    var check = "SELECT * FROM [ASusers] WHERE Email = @Email";
                    string gethash = "SELECT PasswordHash FROM [ASusers] WHERE Email = @Email";
                    string getsalt = "SELECT PasswordSalt FROM [ASusers] WHERE Email = @Email";
                    string getid = "SELECT Id FROM [ASusers] WHERE Email = @Email";
                    string lockout = "SELECT IsTimeout FROM [ASusers] WHERE Email = @Email";
                    string updateLockout = "UPDATE [ASusers] SET IsTimeout = @IsTimeout WHERE Email = @Email";
                    string lockoutTime = "SELECT Timeout FROM [ASusers] WHERE Email = @Email";
                    string setLockoutTime = "UPDATE [ASusers] SET Timeout = @Timeout WHERE Email = @Email";
                    using (var cmd = new SqlCommand(check, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", Email.Text);
                        bool exist = Convert.ToBoolean(cmd.ExecuteScalar());
                        if (!exist)
                        {
                            errorMsg.Text = "Email not registered";
                            errorMsg.ForeColor = Color.Red;
                            errorMsg.Visible = true;
                            Session["Attempts"] = Convert.ToInt32(Session["Attempts"]) + 1;
                            if (Convert.ToInt32(Session["Attempts"]) >= 3)
                            {
                                //implement timeout
                                timeout = DateTime.Now.ToString();
                                Session["Timeout"] = timeout;

                            }
                            return;
                        }
                        else
                        {

                            using (var checkTimout = new SqlCommand(lockoutTime, con))
                            {
                                checkTimout.Parameters.AddWithValue("@Email", email);
                                var timeout = checkTimout.ExecuteScalar().ToString();
                                var timeoutDate = Convert.ToDateTime(timeout);
                                var timenow = DateTime.Now;
                                var result = DateTime.Compare(timenow, timeoutDate);
                                if (result == -1)
                                {
                                    errorMsg.Text = "Account has been locked, please try again later";
                                    errorMsg.ForeColor = Color.Red;
                                    return;
                                }
                            }
                        }
                    }
                    using (var cmd = new SqlCommand(gethash, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        hash = cmd.ExecuteScalar().ToString();

                    }
                    using (var cmd = new SqlCommand(getsalt, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        salt = cmd.ExecuteScalar().ToString();
                    }
                    using (var cmd = new SqlCommand(getid, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        id = cmd.ExecuteScalar().ToString();
                    }
                    SHA512Managed hashing = new SHA512Managed();
                    var pwdWithSalt = Password.Text + salt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    var finalHash = Convert.ToBase64String(hashWithSalt);
                    if (hash == finalHash)
                    {
                        Session["LoggedIn"] = email;
                        Session["UserId"] = id;
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        Response.Redirect("home.aspx", false);
                    }
                    else
                    {
                        errorMsg.Text = "Email and Password is/are incorrect";
                        errorMsg.ForeColor = Color.Red;
                        Session["Attempts"] = Convert.ToInt32(Session["Attempts"]) + 1;
                        Debug.WriteLine(Session["Attempts"].ToString());

                        if (Convert.ToInt32(Session["Attempts"]) >= 3)
                        {
                            timeout = DateTime.Now.AddMinutes(5).ToString();
                            Session["Timeout"] = timeout;

                        }
                        using (var cmd = new SqlCommand(check, con))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            bool exist = Convert.ToBoolean(cmd.ExecuteScalar());
                            if (exist)

                                using (var lockoutcmd = new SqlCommand(lockout, con))
                                {
                                    lockoutcmd.Parameters.AddWithValue("@Email", email);
                                    var isLockedOut = Convert.ToInt32(lockoutcmd.ExecuteScalar().ToString());
                                    if (isLockedOut == 4)
                                    {
                                        using (var setnewlockouttime = new SqlCommand(setLockoutTime, con))
                                        {
                                            var newTime = DateTime.Now.AddMinutes(30).ToString();
                                            setnewlockouttime.Parameters.AddWithValue("@Email", email);
                                            setnewlockouttime.Parameters.AddWithValue("@Timeout", newTime);
                                        }
                                        errorMsg.Text = "Account has been locked, please try again later";
                                        errorMsg.ForeColor = Color.Red;
                                        using (var setlockout = new SqlCommand(updateLockout, con))
                                        {
                                            var newLockOut = 0;
                                            setlockout.Parameters.AddWithValue("@IsTimeout", newLockOut);
                                            setlockout.Parameters.AddWithValue("@Email", email);
                                        }
                                        return;
                                    }
                                    using (var setlockout = new SqlCommand(updateLockout, con))
                                    {
                                        var newLockOut = isLockedOut + 1;
                                        setlockout.Parameters.AddWithValue("@IsTimeout", newLockOut);
                                        setlockout.Parameters.AddWithValue("@Email", email);
                                    }
                                }

                        }
                        con.Close();
                    }

                }



            }
            
           
                
            
        }
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
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