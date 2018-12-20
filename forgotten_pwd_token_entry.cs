using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace final_development
{
    public partial class forgotten_pwd_token_entry : Form
    {
        public forgotten_pwd_token_entry()
        {
            InitializeComponent();
        }

        private void forgot_pwd_btn_Click(object sender, EventArgs e)
        {
            string user_email = forgotten_email_textbox.Text.Trim();
            string user_pass = forgot_pwd_emailpass_textbox.Text.Trim();
            database_class db_object = new database_class();
            if (found_email(user_email, db_object))
            {
                DataTable id_table = db_object.obtain_matching_user_id_for_email(user_email);
                int id = 0;
                foreach (DataRow rows in id_table.Rows)
                {
                    id = Convert.ToInt16(rows["user_id"]);
                }
                int random_number = random_no_function(1, 100);
                string hashed_random_number = hash(random_number.ToString());
                string hashed_id = hash(id.ToString());
                string concatenated_hash_value = hashed_random_number + hashed_id;
                string token = hash(concatenated_hash_value);
                try
                {
                    db_object.email_hash_and_add_to_record(token, id, user_email, 
                                                            user_pass);
                }
                catch (Exception the_exception)
                {
                    MessageBox.Show("Could not send email. Are you sure it is a 'gmail' account?");
                }
                
            }
        }

        public bool found_email(string user_email, database_class setup_object)
        {
            DataTable emails_table = setup_object.obtain_all_usernames_and_emails();
            bool found_email = false;
            foreach (DataRow row in emails_table.Rows)
            {
                if (user_email == row["email_address"].ToString())
                {
                    found_email = true;
                }
                if (found_email)
                {
                    return true;
                }
            }

            MessageBox.Show("Email not found");
            return false;
        }

        public int random_no_function(int lowest_val, int highest_val)
        {
            Random random_object = new Random();
            int random_number = random_object.Next(lowest_val, highest_val);
            return random_number;
        }

        private string hash(string the_string)
        {
            // http://dotnet-snippets.com/snippet/generate-md5-hash/644
            MD5 crytographic_md5_hash_function = new MD5CryptoServiceProvider();
            byte[] item_to_hash = Encoding.Default.GetBytes(the_string);
            byte[] hashed_result = crytographic_md5_hash_function.ComputeHash(item_to_hash);
            return System.BitConverter.ToString(hashed_result);
        }

        private void back_login_btn_Click(object sender, EventArgs e)
        {
            Login_Register_Form login = new Login_Register_Form();
            login.Show();
            this.Hide();
            this.Dispose();
            
        }

        private static bool empty_input(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return true;
            }
            return false;
        }
        private void token_btn_Click(object sender, EventArgs e)
        {
            string user_email = got_token_email_textbox.Text.Trim();
            string user_token = got_token_token_textbox.Text.Trim();
            if (!empty_input(user_email, user_token))
            {
                database_class db_object = new database_class();
                DataTable id_table = db_object.obtain_matching_user_id_for_email(user_email);
                string user_id = "0"; // temporarily initialised
                foreach(DataRow row in id_table.Rows)
                {
                    user_id = row["user_id"].ToString();
                }
                DataTable matching_token_table = db_object.obtain_matching_token(user_id);
                string matching_token = "";
                foreach (DataRow row in matching_token_table.Rows)
                {
                    matching_token = row["token"].ToString();
                }
                if (user_token == matching_token)
                {
                    MessageBox.Show("Token correct");
                    password_form the_password_form = new password_form();
                    the_password_form.Show();
                    the_password_form.user_id = user_id;
                    this.Hide();
                    this.Dispose();
                    

                }
                else
                {
                    MessageBox.Show("Incorrect token entered");
                }

            }
            else
            {
                MessageBox.Show("Please enter a token");
            }
            
        }
    }
}
