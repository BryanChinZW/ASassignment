using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASassignment
{
    public partial class home : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["LoggedIn"] == null)
            {
               
            }
            var email = Session["LoggedIn"].ToString();
            var user = new User();
            var theUser = user.getUser(email);
            var creditName = Convert.FromBase64String(theUser.CreditName);
            lbl_CreditName.Text = decryptData(theUser.IV,theUser.Key,creditName).Trim();
            var creditNo = Convert.FromBase64String(theUser.CreditNo);
            lbl_CreditNo.Text = decryptData(theUser.IV, theUser.Key, creditNo).Trim();
            var creditDate = Convert.FromBase64String(theUser.CreditDate);
            lbl_creditDate.Text = decryptData(theUser.IV, theUser.Key, creditDate);
            var cvv = Convert.FromBase64String(theUser.CVV);
            lbl_CVV.Text = decryptData(theUser.IV, theUser.Key, cvv);
            var time = Convert.ToDateTime(getDate(email));
            time.AddMinutes(15);
            var passwordage = DateTime.Compare(time, DateTime.Now);
            if (passwordage > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ok", "alertMessage();", true);
            }

        }
        protected string decryptData(byte[] iv, byte[] key, byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = iv;
                cipher.Key = key;
                cipher.Padding = PaddingMode.Zeros;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                //Create the streams used for decryption

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            //Read the decrypted bytes from the decrypting stream
                            //and place them in a string
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected void SignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("Login.aspx");
        }
        public string getDate(string email)
        {
            string queryDate = "SELECT TimeCreated FROM [PasswordHistory] WHERE Email = @paraEmail";
            SqlConnection con = new SqlConnection(MYDBConnectionString);
            SqlCommand cmd = new SqlCommand(queryDate, con);
            cmd.Parameters.AddWithValue("@paraEmail", email);
            con.Open();
            string date = (string)cmd.ExecuteScalar();
            con.Close();
            return date;

        }

        protected void passChange_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx");
        }
    }
    public class User
    {
        public string Email { get; set; }
        public string CreditName { get; set; }
        public string CreditNo { get; set; }
        public string CreditDate { get; set; }
        public string CVV { get; set; }
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
        public User()
        {

        }
        public User(string email, string creditname, string creditno, string creditdate, string cvv, byte[] iv, byte[] key)
        {
            Email = email;
            CreditName = creditname;
            CreditNo = creditno;
            CreditDate = creditdate;
            CVV = cvv;
            IV = iv;
            Key = key;
        }
        public User getUser(string email)
        {
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString);

            //Step 2 -  Create a DataAdapter to retrieve data from the database table
            string sqlStmt = "Select * from [ASusers] where Email = @paraEmail";
            SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
            da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

            //Step 3 -  Create a DataSet to store the data to be retrieved
            DataSet ds = new DataSet();

            //Step 4 -  Use the DataAdapter to fill the DataSet with data retrieved
            da.Fill(ds);
            User user = null;
            int rec_cnt = ds.Tables[0].Rows.Count;
            if (rec_cnt == 1)
            {
                DataRow row = ds.Tables[0].Rows[0];  // Sql command returns only one record
                string CreditName = row["CreditName"].ToString();
                string CreditNo = row["CreditNo"].ToString();
                string CreditDate = row["CreditDate"].ToString();
                string CVV = row["CVV"].ToString();
                byte[] IV = Convert.FromBase64String(row["IV"].ToString());
                byte[] Key = Convert.FromBase64String(row["Key"].ToString());
                user = new User(email,CreditName, CreditNo, CreditDate, CVV, IV, Key);

            }
            return user;
        }
    }

}