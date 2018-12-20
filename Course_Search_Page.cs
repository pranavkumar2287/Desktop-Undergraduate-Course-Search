using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace final_development
{
    public partial class Course_Search_Page : Form
    {
        public Course_Search_Page()
        {
            InitializeComponent();
            //database_class course_obj = new database_class();
            //DataTable all_saved_courses = course_obj.obtain_all_saved_courses();
        }
        public string current_user_id;
        DataTable results_table = new DataTable();
        public string current_username;

        
        public bool valid_inputs(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
            {
                MessageBox.Show("One or more empty text-fields. Please try your search again");
                return false;
            }
            return true;
        }

        private void course_search_btn_Click(object sender, EventArgs e)
        {
            result_panel.Controls.Clear(); // new addition to clear panel
            results_table.Clear(); // another new addition to clear actual results table
            numericUpDown1.Maximum = 500000; // since there are never going to be 500 thousand pages 
            numericUpDown1.Value = 1; /// numeric up down always set to 1 at each search
            no_of_results_lbl.Text = "Searching...";
            no_of_results_lbl.Visible = true;
            Refresh();
            Show();
            page_viewing_lbl1.Visible = true;
            page_viewing_lbl2.Visible = true;
            page_viewing_lbl3.Visible = true;
            numericUpDown1.Visible = true;
            view_btn.Visible = true;
            string user_course_choice = course_name_textbox.Text.ToLower().Trim();
            string user_uni_choice = uni_name_textbox.Text.ToLower().Trim();
            //if (valid_inputs(user_course_choice, user_uni_choice))
            
                database_class course_db_object = new database_class();


                bool sandwich_checked = false;
                bool foundation_checked = false;
                bool year_abroad_checked = false;
                bool honours_degree_checked = false;
                bool full_time_checked = false;
                bool part_time_checked = false;
                bool fee_waiver_checked = false;
                bool fixed_fee_checked = false;
                bool tef_gold_checked = false;
                bool tef_silver_checked = false;
                bool tef_bronze_checked = false;

                if (sandwich_checkbox.Checked)
                {
                    sandwich_checked = true;
                }
                if (foundation_checkbox.Checked)
                {
                    foundation_checked = true;
                }
                if (yrabroad_checkbox.Checked)
                {
                    year_abroad_checked = true;
                }
                if (honours_checkbox.Checked)
                {
                    honours_degree_checked = true;
                }
                if (fulltime_checkbox.Checked)
                {
                    full_time_checked = true;
                }
                if (parttime_checkbox.Checked)
                {
                    part_time_checked = true;
                }
                if (fee_waiver_checkbox.Checked)
                {
                    fee_waiver_checked = true;
                }
                if (fixed_fees_checkbox.Checked)
                {
                    fixed_fee_checked = true;
                }
                if (tef_groupbox.Enabled)
                {
                    if (tefgold_radiobtn.Checked)
                    {
                        tef_gold_checked = true;
                    }
                    if (tefsilver_radiobtn.Checked)
                    {
                        tef_silver_checked = true;
                    }
                    if (tefbronze_radiobtn.Checked)
                    {
                        tef_bronze_checked = true;
                    }
                }






            ///DataTable results_table = new DataTable();
            if (string.IsNullOrWhiteSpace(user_uni_choice)) // only course entered
            {
                results_table = course_db_object.course_only_query(user_course_choice, current_user_id, course_db_object,
                                                                    results_table, sandwich_checked, foundation_checked,
                                                                    year_abroad_checked, honours_degree_checked, full_time_checked,
                                                                    part_time_checked, fee_waiver_checked, fixed_fee_checked,
                                                                    tef_gold_checked, tef_silver_checked, tef_bronze_checked);
            }
            else if (string.IsNullOrWhiteSpace(user_course_choice) && string.IsNullOrWhiteSpace(user_uni_choice)) // nothing entered
            {
                results_table = course_db_object.no_enter_query( current_user_id, course_db_object,
                                                                   results_table, sandwich_checked, foundation_checked,
                                                                   year_abroad_checked, honours_degree_checked, full_time_checked,
                                                                   part_time_checked, fee_waiver_checked, fixed_fee_checked,
                                                                   tef_gold_checked, tef_silver_checked, tef_bronze_checked);
            }

            else if (string.IsNullOrWhiteSpace(user_course_choice)) // only university entered
            {
                results_table = course_db_object.uni_only_query(user_uni_choice, current_user_id, course_db_object,
                                                                    results_table, sandwich_checked, foundation_checked,
                                                                    year_abroad_checked, honours_degree_checked, full_time_checked,
                                                                    part_time_checked, fee_waiver_checked, fixed_fee_checked,
                                                                    tef_gold_checked, tef_silver_checked, tef_bronze_checked);
            }
            else // so, both course and university must have been entered - in this case
            {
                results_table = course_db_object.course_and_uni_query(user_course_choice, user_uni_choice, results_table, sandwich_checked,
                                                                    foundation_checked, year_abroad_checked, honours_degree_checked,
                                                                    full_time_checked, part_time_checked, fee_waiver_checked,
                                                                    fixed_fee_checked, tef_gold_checked, tef_silver_checked,
                                                                    tef_bronze_checked);
            }

                int no_of_results = results_table.Rows.Count;
                switch (no_of_results)
                {
                    case 0:
                        no_of_results_lbl.Text = "No Results Returned. Check your search again";
                        break;
                    case 1:
                        no_of_results_lbl.Text = "1 Result Returned";
                        break;
                    default:
                        no_of_results_lbl.Text = no_of_results.ToString() + " Results Returned";
                        break;
                }
                Refresh();
                Show();

                add_to_panel(results_table, no_of_results, course_db_object); // creates the cards and adds to panel

                int no_of_viewing_pages;
                int quotient = Convert.ToInt32(no_of_results) / 10;
                int remainder = no_of_results % 10;
                if (remainder == 0)
                {
                    no_of_viewing_pages = quotient;
                }
                else
                {
                    no_of_viewing_pages = quotient + 1;
                }
                page_viewing_lbl3.Text = "of " + no_of_viewing_pages.ToString() + " pages";
                numericUpDown1.Minimum = 1;
                numericUpDown1.Maximum = no_of_viewing_pages;
            
        }

            public void add_to_panel(DataTable results_table, int no_of_results, database_class course_db_object)
        {
            if (no_of_results <= 10)
            {
                foreach (DataRow kiscourse_results_row in results_table.Rows)
                {
                    Refresh(); // this just improves the user-experience in the windows form
                    Show(); // same purpose as refresh()
                    var result_card = new course_card();
                    result_card.set_values(kiscourse_results_row["KISCOURSEID"], kiscourse_results_row["view_name"].ToString(),
                                            kiscourse_results_row["title"].ToString(),
                                           kiscourse_results_row["website_url"].ToString(), kiscourse_results_row["crseurl"].ToString(),
                                           kiscourse_results_row["assurl"].ToString(),
                                           kiscourse_results_row["sandwich"], kiscourse_results_row["yearabroad"],
                                           kiscourse_results_row["foundation"],
                                           kiscourse_results_row["honours"], kiscourse_results_row["kisaimlabel"],
                                           kiscourse_results_row["avgwritten"], kiscourse_results_row["avgcoursework"],
                                           kiscourse_results_row["avgscheduled"], current_user_id, course_db_object.obtain_all_saved_courses(),
                                           kiscourse_results_row["UKPRN"].ToString());
                    result_panel.Controls.Add(result_card);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {

                    var result_card = new course_card();
                    result_card.set_values(results_table.Rows[i]["KISCOURSEID"], results_table.Rows[i]["view_name"].ToString(),
                                            results_table.Rows[i]["title"].ToString(),
                                          results_table.Rows[i]["website_url"].ToString(), results_table.Rows[i]["crseurl"].ToString(),
                                           results_table.Rows[i]["assurl"].ToString(),
                                           results_table.Rows[i]["sandwich"], results_table.Rows[i]["yearabroad"],
                                           results_table.Rows[i]["foundation"],
                                           results_table.Rows[i]["honours"], results_table.Rows[i]["kisaimlabel"],
                                           results_table.Rows[i]["avgwritten"], results_table.Rows[i]["avgcoursework"],
                                           results_table.Rows[i]["avgscheduled"], current_user_id, course_db_object.obtain_all_saved_courses(),
                                           results_table.Rows[i]["UKPRN"].ToString());

                    result_panel.Controls.Add(result_card);
                }

            }
        }

        

        private void view_btn_Click(object sender, EventArgs e)
        {
            result_panel.Controls.Clear(); // NOT clearing results-table, as this method only allows viewing
                                           // different pages of the same results table - in the form of course cards
            database_class another_db_object = new database_class();
            if (numericUpDown1.Value == numericUpDown1.Maximum) // if on the last page
            {
                int low_limit = ((Convert.ToInt16(numericUpDown1.Value) - 1) * 10) + 1;
                int no_of_cards = results_table.Rows.Count - low_limit;
                int flag = 0;
                for (int i = low_limit -1; i < 100; i++ )
                {
                        Refresh(); // this just improves the user-experience in the windows form
                        Show(); // same purpose as refresh()
                        var result_card = new course_card();
                        result_card.set_values(results_table.Rows[i]["KISCOURSEID"], results_table.Rows[i]["view_name"].ToString(),
                                            results_table.Rows[i]["title"].ToString(),
                                            results_table.Rows[i]["website_url"].ToString(), results_table.Rows[i]["crseurl"].ToString(),
                                           results_table.Rows[i]["assurl"].ToString(),
                                           results_table.Rows[i]["sandwich"], results_table.Rows[i]["yearabroad"],
                                           results_table.Rows[i]["foundation"],
                                           results_table.Rows[i]["honours"], results_table.Rows[i]["kisaimlabel"],
                                           results_table.Rows[i]["avgwritten"], results_table.Rows[i]["avgcoursework"],
                                           results_table.Rows[i]["avgscheduled"], current_user_id, another_db_object.obtain_all_saved_courses(),
                                           results_table.Rows[i]["UKPRN"].ToString());
                    result_panel.Controls.Add(result_card);
                        
                        if (flag == no_of_cards)
                    {
                        i = 100; // terminates the while loop, since it would never reach 100 by its own
                    }
                    flag++;
                }
            }
            else
            {
                int low_limit = ((Convert.ToInt16(numericUpDown1.Value) - 1) * 10) + 1;
                int up_limit = Convert.ToInt16(numericUpDown1.Value) * 10;
                for (int i = low_limit - 1; i < up_limit - 1; i++)
                {
                    var result_card = new course_card();
                    result_card.set_values(results_table.Rows[i]["KISCOURSEID"], results_table.Rows[i]["view_name"].ToString(),
                                            results_table.Rows[i]["title"].ToString(),
                                            results_table.Rows[i]["website_url"].ToString(), results_table.Rows[i]["crseurl"].ToString(),
                                           results_table.Rows[i]["assurl"].ToString(),
                                           results_table.Rows[i]["sandwich"], results_table.Rows[i]["yearabroad"],
                                           results_table.Rows[i]["foundation"],
                                           results_table.Rows[i]["honours"], results_table.Rows[i]["kisaimlabel"],
                                           results_table.Rows[i]["avgwritten"], results_table.Rows[i]["avgcoursework"],
                                           results_table.Rows[i]["avgscheduled"], current_user_id, another_db_object.obtain_all_saved_courses(),
                                           results_table.Rows[i]["UKPRN"].ToString());

                    result_panel.Controls.Add(result_card);
                }
            }
            
            

        }

        private void TEF_on_off_btn_Click(object sender, EventArgs e)
        {
            tef_groupbox.Enabled = !tef_groupbox.Enabled;
        }

        private void tef_groupbox_EnabledChanged(object sender, EventArgs e)
        {
            if (tef_groupbox.Enabled)
            {
                tef_enabled_disabled_label.Text = "TEF Filter Enabled";

            }
            else if (tef_groupbox.Enabled == false)
            {
                tef_enabled_disabled_label.Text = "TEF Filter Disabled";
            }
        }

        private void view_saved_courses_btn_Click(object sender, EventArgs e)
        {
            results_table.Clear();
            result_panel.Controls.Clear();
            database_class saved_courses_db_object = new database_class();
            DataTable all_kiscourseids = saved_courses_db_object.specific_saved_courses(current_user_id);
            foreach (DataRow row in all_kiscourseids.Rows)
            {
                results_table = saved_courses_db_object.kiscourse_course_query(saved_courses_db_object, results_table, row["kiscourseid"].ToString(),
                                                                                row["ukprn"].ToString());
            }
            add_to_panel(results_table, results_table.Rows.Count, saved_courses_db_object);
            int no_of_viewing_pages;
            int quotient = Convert.ToInt16(results_table.Rows.Count) / 10;
            int remainder = results_table.Rows.Count % 10;
            if (remainder == 0)
            {
                no_of_viewing_pages = quotient;
            }
            else
            {
                no_of_viewing_pages = quotient + 1;
            }
            page_viewing_lbl3.Text = "of " + no_of_viewing_pages.ToString() + " pages";
            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = no_of_viewing_pages;
            if (results_table.Rows.Count == 0)
            {
                Refresh();
                Show();
                no_of_results_lbl.Text = "No saved courses";
            }
            else if (results_table.Rows.Count == 1)
            {
                no_of_results_lbl.Text = "1 Course Returned";
            }
            else
            {
                no_of_results_lbl.Text = results_table.Rows.Count.ToString() + " Results Returned";
            }
            



        }

        private void log_out_btn_Click(object sender, EventArgs e)
        {
            if (current_user_id == 0.ToString())
            {
                database_class log_out_db_object = new database_class();
                log_out_db_object.delete_guest_courses(); // still need to add method                     
            }
            Login_Register_Form login_form = new Login_Register_Form();
            login_form.Show();
            this.Dispose();
            this.Hide();

        }

        private void Course_Search_Page_Load(object sender, EventArgs e)
        {
            if (current_user_id == 0.ToString())
            {
                greeting_lbl.Text = "Welcome, Guest";
            }
            else
            {
                greeting_lbl.Text = "Welcome, " + current_username;
            }
            
            Refresh();
            Show();
        }
    }
}
