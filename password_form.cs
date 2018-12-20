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
    public partial class password_form : Form
    {
        public password_form()
        {
            InitializeComponent();
        }
        public string user_id;

        private string hash(string plaintext)
        {
            // http://dotnet-snippets.com/snippet/generate-md5-hash/644
            MD5 crytographic_md5_hash_function = new MD5CryptoServiceProvider();
            byte[] plaintext_to_hash = Encoding.Default.GetBytes(plaintext);
            byte[] hashed_result = crytographic_md5_hash_function.ComputeHash(plaintext_to_hash);
            return System.BitConverter.ToString(hashed_result);
        }


        private void change_btn_Click(object sender, EventArgs e)
        {
            if (new_pwd_textbox.Text == confirm_pwd_textbox.Text)
            {
                database_class login_object = new database_class();
                string hashed_id = hash(user_id);
                string hashed_password = hash(new_pwd_textbox.Text);
                string total = hashed_password + hashed_id;
                string hashed_total = hash(total);
                login_object.change_password(user_id, hashed_total);
            }
            else
            {
                MessageBox.Show("Passwords do not match!");
            }
           
        }

        private void back_login_btn_Click(object sender, EventArgs e)
        {
            Login_Register_Form login_form = new Login_Register_Form();
            login_form.Show();
            this.Hide();
            this.Dispose();
        }

        private void back_token_btn_Click(object sender, EventArgs e)
        {
            forgotten_pwd_token_entry forgotten_pwd_and_token_form 
            = new forgotten_pwd_token_entry();
            forgotten_pwd_and_token_form.Show();
            this.Hide();
            this.Dispose();
        }
    }
}
