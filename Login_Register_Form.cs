using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace final_development
{
    public partial class Login_Register_Form : Form
    {
        public Login_Register_Form()
        {
            InitializeComponent();
        }

        private void login_go_btn_Click(object sender, EventArgs e)
        {
            string username = login_username_input.Text.Trim();
            string password = login_password_input.Text.Trim();
            if (valid_inputs(username, password))
            {
                database_class db_object = new database_class(); //1 instantiate
                if (username_found(username, db_object))
                {
                    string user_id = get_id(username, db_object);
                    string final_hash = hash_id_and_username(user_id, password);
                    bool correct_login = check_password_match(user_id, final_hash, db_object);
                    if (correct_login)
                    {
                        Course_Search_Page course_page = new Course_Search_Page();
                        course_page.current_user_id = user_id;
                        course_page.current_username = username;
                        course_page.Show();
                        this.Hide();
                    }
                }
                // else failed login
            }
        }

        public bool check_password_match(string user_id, string final_hash, database_class db_object)
        {
            DataTable matching_password_hashed_table = db_object.obtain_matching_hashed_password(user_id);
            foreach (DataRow row in matching_password_hashed_table.Rows)
            {
                if (final_hash == row["hashed_password_with_salt"].ToString())
                {
                    MessageBox.Show("Login successful");
                    return true;
                }
                else
                {
                    MessageBox.Show("Failed login - Invalid Credentials");
                    return false;
                }
            }
            return false; // default return value
        }

        public string hash_id_and_username(string user_id, string password)
        {
            string hashed_id = hash(user_id); // hash using my custom definition
            string hashed_password = hash(password);
            string concatenated_id_password = hashed_password + hashed_id;
            string final_hash = hash(concatenated_id_password);
            return final_hash;
        }

        public string get_id(string username, database_class db_object)
        {
            DataTable matching_id_table = db_object.obtain_matching_user_id(username);
            // call the class method to store matching id in a DataTable, to iterate through
            string matching_id = ""; 
            // matching_id must be initialised, else program cannot return a potential 'empty' string 
            foreach (DataRow row in matching_id_table.Rows) 
            // there should only be 1 matching id, due to validation in registration
            {
                matching_id = row["user_id"].ToString(); 
            }
            return matching_id;
        }
       
        public string hash(string plaintext)
        {
            MD5 crytographic_md5_hash_function = new MD5CryptoServiceProvider();
            byte[] plaintext_bytes = Encoding.Default.GetBytes(plaintext);
            byte[] hashed_result = crytographic_md5_hash_function.ComputeHash(plaintext_bytes);
            return System.BitConverter.ToString(hashed_result);
        }
        
        public bool username_found(string username, database_class db_object)
        {
            DataTable all_usernames_table = db_object.obtain_all_usernames(); // call method to return all usernames 
            bool username_found = false; // a flag variable
            foreach (DataRow row in all_usernames_table.Rows) // iterate through each datarow of the table's rows 
            {
                if (username == row["username"].ToString()) // only happens once, since there cannot be more than one matching username in table
                {
                    username_found = true; // set the flag variable to true
                }
                if (username_found == true) // if flag variable is true
                {
                    return true; // implies the username is matched and found
                }
            }
            MessageBox.Show("Username" + username + " not found"); // at this stage, no matching username was found - informing user
            return false; // so do not return true
        }
            
        public bool valid_inputs(string input1, string input2, string input3 = "a", string input4 = "a")
        {
            if (string.IsNullOrWhiteSpace(input1) && string.IsNullOrWhiteSpace(input2)
                && string.IsNullOrWhiteSpace(input3) && string.IsNullOrWhiteSpace(input4))
            {
                return false;
            }
            return true;
        }

        private void register_submit_btn_Click(object sender, EventArgs e)
        {
            string new_username = register_new_username_input.Text.Trim();
            string new_password = register_password_input.Text.Trim();
            string confirm_password = register_confirm_password_input.Text.Trim();
            string email = register_email_input.Text.Trim();
            if (valid_inputs(new_username, new_password, confirm_password, email))
            {
                if (matching_passwords(new_password, confirm_password) && valid_email(email))
                {
                    setup_connection(new_username, new_password, confirm_password, email);
                    register_new_username_input.Text = string.Empty;
                    register_password_input.Text = string.Empty;
                    register_confirm_password_input.Text = string.Empty;
                    register_email_input.Text = string.Empty;   
                }
            }
            else
            {
                MessageBox.Show("Multiple empty inputs found. Please try again");
            }
            
        }

        private bool no_matching_usernames_emails(DataTable username_email_table, string current_username, string current_email, bool existing_username_found, bool existing_email_found)
        {
            foreach (DataRow row in username_email_table.Rows)
            {
                string table_username = row["username"].ToString();
                if (current_username == table_username)
                {
                    existing_username_found = true;
                }
                string table_email = row["email_address"].ToString();
                if (current_email == table_email)
                {
                    existing_email_found = true;
                }
            }
            if (existing_username_found)
            {
                MessageBox.Show("Username already exists");
                return false;
            }
            if (existing_email_found)
            {
                MessageBox.Show("Email already exists");
                return false;
            }
            return true;
        }

        private void setup_connection(string new_username, string new_password, string confirm_password, string email)
        {
            database_class register_db_object = new database_class();
            DataTable all_usernames_and_emails_table = register_db_object.obtain_all_usernames_and_emails();
            bool existing_username_found = false;
            bool existing_email_found = false;
            if (no_matching_usernames_emails(all_usernames_and_emails_table, new_username, email, existing_username_found, existing_email_found))
            {
                register_db_object.create_blank_record();
                DataTable id_table = register_db_object.get_last_userid_of_last_row();
                int the_user_id = 0;
                foreach(DataRow current_row in id_table.Rows)
                {
                    the_user_id = Convert.ToInt16(current_row["user_id"]);
                }
                string final_hash_value = hash_id_and_username(the_user_id.ToString(), new_password);
                register_db_object.add_new_user(new_username, final_hash_value, email, the_user_id);
                MessageBox.Show("New user added - You can now login");



            }
        }

        private bool matching_passwords(string password1, string password2)
        {
            if (password1 == password2)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Passwords do not match!");
                return false;
            }
        }

        private bool valid_email(string email)
        {
            //http://www.c-sharpcorner.com/blogs/validate-email-address-in-c-sharp1
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Invalid Email");
                return false;
            }
        }

        private void search_as_guest_btn_Click(object sender, EventArgs e)
        {
            Course_Search_Page course_page = new Course_Search_Page();
            course_page.current_user_id = 0.ToString();
            course_page.current_username = "Guest";
            database_class db_object = new database_class();
            db_object.delete_guest_courses(); // deletes the existing guest courses here now
            Refresh();
            Show();
            course_page.Show();
            this.Hide();
        }

        private void forgotten_pwd_lbl_Click(object sender, EventArgs e)
        {
           forgotten_pwd_token_entry new_page = new forgotten_pwd_token_entry();
            new_page.Show();
            this.Hide();
            
        }

        private void got_token_lbl_Click(object sender, EventArgs e)
        {
            forgotten_pwd_token_entry new_page = new forgotten_pwd_token_entry();
            new_page.Show();
            this.Hide();
            
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 0;
            Environment.Exit(0);
        }
    }
}
