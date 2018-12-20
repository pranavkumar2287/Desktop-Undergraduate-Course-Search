using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Printing;

namespace final_development
{
    public partial class course_card : UserControl
    {
        public course_card()
        {
            InitializeComponent();
        }
        public string kis_course_id;
        public string user_id;
        public string ukprn;
        public bool saveable = true;
       
        
        
        
        public void set_values(object kiscourseid, string uni_name, string course_name, string uni_website_url, 
                              string course_url, string assessment_url, object sandwich, object year_abroad, 
                            object foundation, object honours, object kisaimlabel, object avg_written, 
                            object avgcoursework, object avgscheduled, string current_user_id, DataTable all_saved_courses,
                            object ukprn_p)
        {
            bool found_matching_saved_course = false;
            for (int i = 0; i < all_saved_courses.Rows.Count; i++)
            {
                if (current_user_id == all_saved_courses.Rows[i]["USER_ID"].ToString() 
                    && ukprn_p.ToString() == all_saved_courses.Rows[i]["UKPRN"].ToString() 
                    && kiscourseid.ToString() == all_saved_courses.Rows[i]["KISCOURSEID"].ToString())
                {
                    found_matching_saved_course = true;
                }
            }
            if (found_matching_saved_course) // cannot save
            {
                saveable = false;
                save_btn.Text = "Unsave";
            }
           

            user_id = current_user_id.ToString();
            kis_course_id = kiscourseid.ToString();
            ukprn = ukprn_p.ToString();
            uni_name_lbl.Text = uni_name;
            course_name_label.Text = course_name;
            uni_website_link.Text = uni_website_url;
            course_page_link.Text = course_url;
            assessment_link.Text = assessment_url;
            classification_lbl.Text = kisaimlabel.ToString();
            avg_written_lbl.Text = avg_written.ToString() + "%";
            avg_cw_lbl.Text = avgcoursework.ToString() + "%";

            switch (Convert.ToSByte(sandwich))
            {
                case 0:
                    sandwich_lbl.Text = "Sandwich Year not available";
                    break;
                case 1:
                    sandwich_lbl.Text = "Optional Sandwich Year";
                    sandwich_lbl.Font = new Font(sandwich_lbl.Font, FontStyle.Bold);
                    break;
                case 2:
                    sandwich_lbl.Text = "Compulsory Sandwich Year";
                    sandwich_lbl.Font = new Font(sandwich_lbl.Font, FontStyle.Bold);
                    break;
                default:
                    sandwich_lbl.Text = "No data on sandwich year available";
                    break;
            }
            switch (Convert.ToSByte(year_abroad))
            {
                case 0:
                    yr_abroad_lbl.Text = "Year abroad not available";
                    break;
                case 1:
                    yr_abroad_lbl.Text = "Optional Year Abroad";
                    yr_abroad_lbl.Font = new Font(yr_abroad_lbl.Font, FontStyle.Bold);
                    break;
                case 2:
                    yr_abroad_lbl.Text = "Compulsory Year Abroad";
                    yr_abroad_lbl.Font = new Font(yr_abroad_lbl.Font, FontStyle.Bold);
                    break;
                default:
                    yr_abroad_lbl.Text = "No data on Year Abroad available";
                    break;
            }
            switch (Convert.ToSByte(foundation))
            {
                case 0:
                    foundation_yr_lbl.Text = "Foundation-Year not available";
                    break;
                case 1:
                    foundation_yr_lbl.Text = "Optional Foundation Year";
                    foundation_yr_lbl.Font = new Font(foundation_yr_lbl.Font, FontStyle.Bold);
                    break;
                case 2:
                    foundation_yr_lbl.Text = "Compulsory Foundation Year";
                    foundation_yr_lbl.Font = new Font(foundation_yr_lbl.Font, FontStyle.Bold);
                    break;
                default:
                    foundation_yr_lbl.Text = "No data on Year Abroad available";
                    break;
            }
            switch (Convert.ToSByte(honours))
            {
                case 0:
                    honours_degree_lbl.Text = "Not an honours degree";
                    break;
                case 1:
                    honours_degree_lbl.Text = "Honours Degree";
                    
                    break;
                default:
                    honours_degree_lbl.Text = "No data on Honours-Degree available";
                    break;
            }

           
            
        }

        private void uni_website_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("chrome", @"" + uni_website_link.Text + "");
        }

        private void course_page_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("chrome", @"" + course_page_link.Text + "");
        }

        private void assessment_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("chrome", @"" + assessment_link.Text + "");
        }

       
        private void print_btn_Click(object sender, EventArgs e)
        {
            print_btn.Visible = false; // so the buttons do not appear in the printout
            save_btn.Visible = false;
            // Creating a document object
            PrintDocument the_document_object = new PrintDocument();
            // Adding the  print handler
            the_document_object.PrintPage += new PrintPageEventHandler(Document_PrintPage);
            // Create the windows dialog in order to display results
            PrintPreviewDialog dialogue_object = new PrintPreviewDialog();
            dialogue_object.ClientSize = new System.Drawing.Size(Width / 2, Height / 2);
            dialogue_object.Location = new System.Drawing.Point(Left, Top);
            dialogue_object.MinimumSize = new System.Drawing.Size(375, 500); //375, 250
            dialogue_object.UseAntiAlias = true;
            // document set up
            dialogue_object.Document = the_document_object;
            // Showing document
            dialogue_object.ShowDialog(this);
            // Effectively deleting the dialogue to free up some memory
            dialogue_object.Dispose();
        }




        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Creating a bitmap object in matching the size of the form
            Bitmap bitmap_object = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            this.DrawToBitmap(bitmap_object, this.DisplayRectangle);
            e.Graphics.DrawImage(bitmap_object, 0, 0);
            // again to free up some memory
            bitmap_object.Dispose();
            print_btn.Visible = true; // making the buttons visible again for user-interactivity
            save_btn.Visible = true;
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            if (saveable)
            {
                database_class card_db_object = new database_class();
                card_db_object.save_course(user_id, kis_course_id, ukprn);
                MessageBox.Show("Course Saved");
                save_btn.Text = "Unsave";
                saveable = false;
            }
            else // not saveable - so button must have unsave - saveable is false
            {
                database_class card_db_object = new database_class();
                card_db_object.unsave_course(user_id, kis_course_id, ukprn);
                MessageBox.Show("Course has been un-saved");
                save_btn.Text = "Save";
                saveable = true;
            }
            

        }

        private void course_name_label_Click(object sender, EventArgs e)
        {

        }

        private void uni_name_lbl_Click(object sender, EventArgs e)
        {

        }

        private void honours_degree_lbl_Click(object sender, EventArgs e)
        {

        }

        private void classification_lbl_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void avg_cw_lbl_Click(object sender, EventArgs e)
        {

        }

        private void avg_written_lbl_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void course_card_Load(object sender, EventArgs e)
        {

        }

        private void sandwich_lbl_Click(object sender, EventArgs e)
        {

        }

        private void yr_abroad_lbl_Click(object sender, EventArgs e)
        {

        }

        private void foundation_yr_lbl_Click(object sender, EventArgs e)
        {

        }

        private void uni_website_lbl_Click(object sender, EventArgs e)
        {

        }

        private void course_website_lbl_Click(object sender, EventArgs e)
        {

        }

        private void assessment_methods_lbl_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
