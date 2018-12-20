    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;

namespace final_development
{
    public class database_class
    {
        private const string connection_string = @"Data Source=.\SQLEXPRESS;" +
                                                  "Database=kis;Trusted_Connection=True;";
        private SqlConnection the_database_connection;

        public database_class()
        {
            the_database_connection = new SqlConnection(connection_string);
            the_database_connection.Open();
        }

        public DataTable obtain_all_usernames() // a public method in the database class to return all usernames
        {
            string obtain_usernames_query = "SELECT username FROM login_db"; // define the query in a string
            SqlCommand obtain_usernames_command = new SqlCommand(obtain_usernames_query, the_database_connection);
            //Create a sqlcommand object with the connection and query arguments
            SqlDataReader usernames_reader = obtain_usernames_command.ExecuteReader();
            // creating a sqldatareader object which reads data from sql server
            DataTable all_usernames_table = new DataTable();
            // create a datatable to hold all the data - the usernames
            all_usernames_table.Load(usernames_reader);
            // load all the data from the reader into the table defined
            return all_usernames_table; // now return the datatable containing the usernames
        }

        public DataTable obtain_matching_user_id(string username)
        {
            string matching_id_query = "SELECT user_id FROM login_db WHERE username = @username";
            SqlCommand matching_id_command = new SqlCommand(matching_id_query, the_database_connection);
            matching_id_command.Parameters.AddWithValue("@username", username);
            SqlDataReader matching_id_reader = matching_id_command.ExecuteReader();
            DataTable user_id_table = new DataTable();
            user_id_table.Load(matching_id_reader);
            return user_id_table;
        }

        public DataTable obtain_matching_hashed_password(string user_id)
        {
            string hashed_password_id_query = "SELECT hashed_password_with_salt FROM login_db WHERE user_id = @user_id";
            SqlCommand hashed_password_command = new SqlCommand(hashed_password_id_query, the_database_connection);
            hashed_password_command.Parameters.AddWithValue("@user_id", user_id);
            SqlDataReader hashed_password_id_reader = hashed_password_command.ExecuteReader();
            DataTable hashed_password_table = new DataTable();
            hashed_password_table.Load(hashed_password_id_reader);
            return hashed_password_table;
        }

        public DataTable obtain_all_usernames_and_emails()
        {
            string username_email_query = "SELECT username, email_address FROM login_db"; // define the query in a string-variable
            SqlCommand username_email_command = new SqlCommand(username_email_query, the_database_connection);
            // Instantiate a new sqlcommand object with the query and connection arguments
            SqlDataReader username_email_reader = username_email_command.ExecuteReader(); // Instantiate a SQL DataReader object which reads SQL data
            DataTable username_email_table = new DataTable(); // Instantiate a datatable object, storage of usernames and emails
            username_email_table.Load(username_email_reader); // Load the data from the SQL DataReader object into the datatable object
            return username_email_table; // return the DataTable with all the usernames and emails in the existing login_db table
        }

        public void create_blank_record()
        {
            string new_record_query = "INSERT INTO login_db (username, hashed_password_with_salt, email_address, token)    " +
                                       " VALUES (NULL, NULL, NULL, NULL);"; // excludes the auto-increment user_id attribute
            SqlCommand new_record_command = new SqlCommand(new_record_query, the_database_connection);
            SqlDataAdapter new_record_adapter = new SqlDataAdapter();
            new_record_adapter.InsertCommand = new_record_command;
            new_record_adapter.InsertCommand.ExecuteNonQuery();
        }

        public DataTable get_last_userid_of_last_row()
        {
            string last_record_query = "SELECT TOP 1  user_id FROM login_db ORDER BY user_id DESC ";
            SqlCommand last_record_command = new SqlCommand(last_record_query, the_database_connection);
            SqlDataReader last_record_id_reader = last_record_command.ExecuteReader();
            DataTable last_record_id_table = new DataTable();
            last_record_id_table.Load(last_record_id_reader);
            return last_record_id_table;
        }

        public void add_new_user(string username, string hashed_password_with_salt, string email, int the_id)
        {
            string add_new_user_query = "UPDATE login_db  " +
                                         "SET username = @user_username, hashed_password_with_salt = @hash_pw_salt, email_address = @user_email  " +
                                         "WHERE user_id = @user_id ;";
            SqlCommand add_new_user_command = new SqlCommand(add_new_user_query, the_database_connection);
            add_new_user_command.Parameters.AddWithValue("@user_username", username);
            add_new_user_command.Parameters.AddWithValue("@hash_pw_salt", hashed_password_with_salt);
            add_new_user_command.Parameters.AddWithValue("@user_email", email);
            add_new_user_command.Parameters.AddWithValue("@user_id", the_id);

            SqlDataAdapter add_new_user_adapter = new SqlDataAdapter();
            add_new_user_adapter.InsertCommand = add_new_user_command;
            add_new_user_adapter.InsertCommand.ExecuteNonQuery();
        }

        public DataTable no_enter_query( string current_user_id, database_class db_object, DataTable results_table,
                                            bool sandwich_checked, bool foundation_checked, bool year_abroad_checked, bool honours_degree_checked,
                                            bool full_time_checked, bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                            bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {
            string course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, " +
                                   " sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel  FROM [dbo].[kiscourse] " +
                                   " left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN  " +
                                    "  left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE ";

            if (tef_gold_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN,  kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE tef_ins.TEFOutcome = 'Gold'  ";
            }
            if (tef_silver_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE  tef_ins.TEFOutcome = 'Silver' ";
            }
            if (tef_bronze_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse]
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE tef_ins.TEFOutcome = 'Bronze' ";
            }
            if (sandwich_checked)
            {
                course_query += " and ((sandwich = 1) or (sandwich = 2))  ";
            }
            if (foundation_checked)
            {
                course_query += " and ((foundation = 1) or (foundation = 2))  ";
            }
            if (year_abroad_checked)
            {
                course_query += " and ((yearabroad = 1) or (yearabroad = 2)) ";
            }
            if (honours_degree_checked)
            {
                course_query += " and (honours = 1) ";
            }
            if (full_time_checked)
            {
                course_query += " and (kismode = 1) ";
            }
            if (part_time_checked)
            {
                course_query += " and (kismode = 2) ";
            }
            if (fee_waiver_checked)
            {
                course_query += " and (waiver = 1) ";
            }
            if (fixed_fee_checked)
            {
                course_query += " and (varfee = 20) ";
            }
            course_query += " ORDER BY TITLE;";
            SqlCommand course_command = new SqlCommand(course_query, the_database_connection);
            SqlDataReader course_query_reader = course_command.ExecuteReader();
            results_table.Load(course_query_reader);
            return results_table;
        }

        public DataTable course_only_query(string user_course_choice, string current_user_id, database_class db_object, DataTable results_table,
                                            bool sandwich_checked, bool foundation_checked, bool year_abroad_checked, bool honours_degree_checked,
                                            bool full_time_checked, bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                            bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {
            string course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, " +
                                   " sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel  FROM [dbo].[kiscourse] " +
                                   " left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN  " +
                                    "  left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE " +
                                    " WHERE TITLE LIKE '%' + @user_course_choice + '%'  ";

            if (tef_gold_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN,  kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and tef_ins.TEFOutcome = 'Gold'  ";
            }
            if (tef_silver_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and tef_ins.TEFOutcome = 'Silver' ";
            }
            if (tef_bronze_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse]
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and tef_ins.TEFOutcome = 'Bronze' ";
            }
            if (sandwich_checked)
            {
                course_query += " and ((sandwich = 1) or (sandwich = 2))  ";
            }
            if (foundation_checked)
            {
                course_query += " and ((foundation = 1) or (foundation = 2))  ";
            }
            if (year_abroad_checked)
            {
                course_query += " and ((yearabroad = 1) or (yearabroad = 2)) ";
            }
            if (honours_degree_checked)
            {
                course_query += " and (honours = 1) ";
            }
            if (full_time_checked)
            {
                course_query += " and (kismode = 1) ";
            }
            if (part_time_checked)
            {
                course_query += " and (kismode = 2) ";
            }
            if (fee_waiver_checked)
            {
                course_query += " and (waiver = 1) ";
            }
            if (fixed_fee_checked)
            {
                course_query += " and (varfee = 20) ";
            }
            course_query += " ORDER BY TITLE;";
            SqlCommand course_command = new SqlCommand(course_query, the_database_connection);
            course_command.Parameters.AddWithValue("@user_course_choice", user_course_choice);
            SqlDataReader course_query_reader = course_command.ExecuteReader();
            results_table.Load(course_query_reader);
            return results_table;
        }






        public DataTable uni_only_query(string user_uni_choice, string current_user_id, database_class db_object, DataTable results_table,
                                            bool sandwich_checked, bool foundation_checked, bool year_abroad_checked, bool honours_degree_checked,
                                            bool full_time_checked, bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                            bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {

            SqlCommand uni_UKPRN_query_command;
            string uni_UKPRN_query = "SELECT UKPRN from learning_providers_plus WHERE provider_name LIKE '%' + @uni_choice + '%' OR view_name " +
                                     "LIKE '%' +  @uni_choice  + '%' OR sort_name LIKE '%' + @uni_choice + '%'";
            uni_UKPRN_query_command = new SqlCommand(uni_UKPRN_query, the_database_connection);
            uni_UKPRN_query_command.Parameters.AddWithValue("@uni_choice", user_uni_choice);
            SqlDataReader uni_UKPRN_reader = uni_UKPRN_query_command.ExecuteReader();
            // Results of UKPRN Query into table
            DataTable UKPRN_table = new DataTable();
            UKPRN_table.Load(uni_UKPRN_reader);


            // add all UKPRNS from new results table, to a list 
            List<int> all_ukprns = new List<int>();
            foreach (DataRow row in UKPRN_table.Rows)
            {
                string current_ukprn = row["UKPRN"].ToString();
                all_ukprns.Add(Convert.ToInt32(current_ukprn));
            }

            for (int i = 0; i <= (UKPRN_table.Rows.Count - 1); i++)
            {
                results_table = uni_query_no_specified_course(results_table, all_ukprns[i],
                                                              the_database_connection, sandwich_checked, foundation_checked,
                                                              year_abroad_checked, honours_degree_checked, full_time_checked,
                                                              part_time_checked, fee_waiver_checked, fixed_fee_checked,
                                                              tef_gold_checked, tef_silver_checked, tef_bronze_checked);
            }

            return results_table;
        }

        public DataTable uni_query_no_specified_course(DataTable results_table, int ukprn, SqlConnection the_database_connection, // invoked by the class method above
                                                       bool sandwich_checked, bool foundation_checked,
                                                       bool year_abroad_checked, bool honours_degree_checked, bool full_time_checked,
                                                       bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                                       bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {

            string course_query = @"SELECT kiscourse.UKPRN,  kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, " +
                                   " sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel  FROM [dbo].[kiscourse] " +
                                   " left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN  " +
                                    "  left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE " +
                                    " WHERE kiscourse.UKPRN =  @ukprn ";

            if (tef_gold_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Gold' ";
            }
            if (tef_silver_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Silver' ";
            }
            if (tef_bronze_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse]
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Bronze' ";
            }
            if (sandwich_checked)
            {
                course_query += " and ((sandwich = 1) or (sandwich = 2))  ";
            }
            if (foundation_checked)
            {
                course_query += " and ((foundation = 1) or (foundation = 2))  ";
            }
            if (year_abroad_checked)
            {
                course_query += " and ((yearabroad = 1) or (yearabroad = 2)) ";
            }
            if (honours_degree_checked)
            {
                course_query += " and (honours = 1) ";
            }
            if (full_time_checked)
            {
                course_query += " and (kismode = 1) ";
            }
            if (part_time_checked)
            {
                course_query += " and (kismode = 2) ";
            }
            if (fee_waiver_checked)
            {
                course_query += " and (waiver = 1) ";
            }
            if (fixed_fee_checked)
            {
                course_query += " and (varfee = 20) ";
            }
            course_query += " ORDER BY TITLE;";
            SqlCommand course_command = new SqlCommand(course_query, the_database_connection);
            course_command.Parameters.AddWithValue("@ukprn", ukprn);
            SqlDataReader course_query_reader = course_command.ExecuteReader();
            results_table.Load(course_query_reader);
            return results_table;
        }








        public DataTable course_and_uni_query(string user_course_choice, string user_uni_choice, DataTable results_table,
                                            bool sandwich_checked, bool foundation_checked, bool year_abroad_checked, bool honours_degree_checked,
                                            bool full_time_checked, bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                            bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {
            SqlCommand uni_UKPRN_query_command;
            string uni_UKPRN_query = "SELECT UKPRN from learning_providers_plus WHERE provider_name LIKE '%' + @uni_choice + '%' OR view_name " +
                                     "LIKE '%' +  @uni_choice  + '%' OR sort_name LIKE '%' + @uni_choice + '%'";
            uni_UKPRN_query_command = new SqlCommand(uni_UKPRN_query, the_database_connection);
            uni_UKPRN_query_command.Parameters.AddWithValue("@uni_choice", user_uni_choice);
            SqlDataReader uni_UKPRN_reader = uni_UKPRN_query_command.ExecuteReader();
            // Results of UKPRN Query into table
            DataTable UKPRN_table = new DataTable();
            UKPRN_table.Load(uni_UKPRN_reader);


            // add all UKPRNS from new results table, to a list 
            List<int> all_ukprns = new List<int>();
            foreach (DataRow row in UKPRN_table.Rows)
            {
                string current_ukprn = row["UKPRN"].ToString();
                all_ukprns.Add(Convert.ToInt32(current_ukprn));
            }

            for (int i = 0; i <= (UKPRN_table.Rows.Count - 1); i++)
            {
                results_table = uni_query_with_specified_course(user_course_choice, results_table, all_ukprns[i],
                                                              the_database_connection, sandwich_checked, foundation_checked,
                                                              year_abroad_checked, honours_degree_checked, full_time_checked,
                                                              part_time_checked, fee_waiver_checked, fixed_fee_checked,
                                                              tef_gold_checked, tef_silver_checked, tef_bronze_checked);
            }

            return results_table;
        }

        public DataTable uni_query_with_specified_course(string user_course_choice, DataTable results_table, int ukprn, // invoked by the method above
                                                              SqlConnection the_database_connection, bool sandwich_checked, bool foundation_checked,
                                                              bool year_abroad_checked, bool honours_degree_checked, bool full_time_checked,
                                                              bool part_time_checked, bool fee_waiver_checked, bool fixed_fee_checked,
                                                              bool tef_gold_checked, bool tef_silver_checked, bool tef_bronze_checked)
        {
            string course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, " +
                                  " sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel  FROM [dbo].[kiscourse] " +
                                  " left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN  " +
                                   "  left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE " +
                                   " WHERE TITLE LIKE '%' + @user_course_choice + '%' and kiscourse.UKPRN =  @ukprn ";

            if (tef_gold_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Gold'  ";
            }
            if (tef_silver_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, title, sort_name, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse] 
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and  kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Silver'  ";
            }
            if (tef_bronze_checked)
            {
                course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel FROM [dbo].[kiscourse]
                                    left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN 
                                     left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE
                                     left join institution as tef_ins on tef_ins.UKPRN = kiscourse.UKPRN                               
                                      WHERE TITLE LIKE '%' + @user_course_choice + '%' and  kiscourse.UKPRN =  @ukprn  and tef_ins.TEFOutcome = 'Bronze'  ";
            }
            if (sandwich_checked)
            {
                course_query += " and ((sandwich = 1) or (sandwich = 2))  ";
            }
            if (foundation_checked)
            {
                course_query += " and ((foundation = 1) or (foundation = 2))  ";
            }
            if (year_abroad_checked)
            {
                course_query += " and ((yearabroad = 1) or (yearabroad = 2)) ";
            }
            if (honours_degree_checked)
            {
                course_query += " and (honours = 1) ";
            }
            if (full_time_checked)
            {
                course_query += " and (kismode = 1) ";
            }
            if (part_time_checked)
            {
                course_query += " and (kismode = 2) ";
            }
            if (fee_waiver_checked)
            {
                course_query += " and (waiver = 1) ";
            }
            if (fixed_fee_checked)
            {
                course_query += " and (varfee = 20) ";
            }
            course_query += " ORDER BY TITLE;";
            SqlCommand course_command = new SqlCommand(course_query, the_database_connection);
            course_command.Parameters.AddWithValue("@user_course_choice", user_course_choice);
            course_command.Parameters.AddWithValue("@ukprn", ukprn);
            SqlDataReader course_query_reader = course_command.ExecuteReader();
            results_table.Load(course_query_reader);
            return results_table;
        }

        public void save_course(string user_id, string kiscourseid, string ukprn)
        {
            string save_course_query = @"INSERT INTO saved_courses(USER_ID, KISCOURSEID, UKPRN)    " +
                                       " VALUES (@id, @courseid, @ukprn);";
            SqlCommand save_course_command = new SqlCommand(save_course_query, the_database_connection);
            save_course_command.Parameters.AddWithValue("@id", user_id);
            save_course_command.Parameters.AddWithValue("@courseid", kiscourseid);
            save_course_command.Parameters.AddWithValue("@ukprn", ukprn);
            SqlDataAdapter save_course_adapter = new SqlDataAdapter();
            save_course_adapter.InsertCommand = save_course_command;
            save_course_adapter.InsertCommand.ExecuteNonQuery();

        }
        public DataTable obtain_all_saved_courses()
        {
            string saved_courses_query = "SELECT * FROM saved_courses";
            SqlCommand saved_courses_command = new SqlCommand(saved_courses_query, the_database_connection);
            SqlDataReader saved_courses_reader = saved_courses_command.ExecuteReader();
            DataTable saved_courses_table = new DataTable();
            saved_courses_table.Load(saved_courses_reader);
            return saved_courses_table;
        }

        public void unsave_course(string user_id, string kiscourseid, string ukprn)
        {
            string unsave_query = @"DELETE FROM saved_courses  " +
                                    "  WHERE USER_ID = @id AND KISCOURSEID = @courseid AND UKPRN = @ukprn";
            SqlCommand unsave_command = new SqlCommand(unsave_query, the_database_connection);
            unsave_command.Parameters.AddWithValue("@id", user_id);
            unsave_command.Parameters.AddWithValue("@courseid", kiscourseid);
            unsave_command.Parameters.AddWithValue("@ukprn", ukprn);
            SqlDataAdapter unsave_adapter = new SqlDataAdapter();
            unsave_adapter.InsertCommand = unsave_command;
            unsave_adapter.InsertCommand.ExecuteNonQuery();
        }

        public DataTable specific_saved_courses(string user_id)
        {
            string saved_courses_query = "SELECT  UKPRN, KISCOURSEID FROM saved_courses WHERE USER_ID = @id";
            SqlCommand saved_courses_command = new SqlCommand(saved_courses_query, the_database_connection);
            saved_courses_command.Parameters.AddWithValue("@id", user_id);
            SqlDataReader saved_courses_reader = saved_courses_command.ExecuteReader();
            DataTable saved_courses_table = new DataTable();
            saved_courses_table.Load(saved_courses_reader);
            return saved_courses_table;
        }

        public DataTable kiscourse_course_query(database_class db_object, DataTable results_table,
                                           string kiscourseid, string ukprn)
        {
            string course_query = @"SELECT kiscourse.UKPRN, kiscourseid, view_name, sort_name, title, website_url, avgwritten, avgcoursework, avgscheduled, yearabroad, " +
                                   " sandwich, assurl, crseurl, employurl, foundation, honours, kismode, lturl, kisaimlabel  FROM [dbo].[kiscourse] " +
                                   " left join learning_providers_plus as unis ON unis.UKPRN = kiscourse.UKPRN  " +
                                    "  left join kisaim as aim_table on aim_table.KISAIMCODE = kiscourse.KISAIMCODE " +
                                    " WHERE KISCOURSEID = @kiscourseid AND kiscourse.ukprn = @ukprn";


            course_query += " ORDER BY TITLE;";
            SqlCommand course_command = new SqlCommand(course_query, the_database_connection);
            course_command.Parameters.AddWithValue("@kiscourseid", kiscourseid);
            course_command.Parameters.AddWithValue("@ukprn", ukprn);
            SqlDataReader course_query_reader = course_command.ExecuteReader();
            results_table.Load(course_query_reader);
            return results_table;
        }
        public void delete_guest_courses()
        {
            string delete_query = @"DELETE FROM saved_courses " + 
                                    " WHERE USER_ID = @id";
            SqlCommand delete_command = new SqlCommand(delete_query, the_database_connection);
            delete_command.Parameters.AddWithValue("@id", 0);
            SqlDataAdapter delete_adapter = new SqlDataAdapter();
            delete_adapter.InsertCommand = delete_command;
            delete_adapter.InsertCommand.ExecuteNonQuery();
        }

        public DataTable obtain_matching_user_id_for_email(string email)
        {
            string query = "SELECT user_id FROM login_db WHERE email_address = @email";
            SqlCommand command = new SqlCommand(query, the_database_connection);
            command.Parameters.AddWithValue("@email", email);
            SqlDataReader the_reader = command.ExecuteReader();
            DataTable user_id_table = new DataTable();
            user_id_table.Load(the_reader);
            return user_id_table;
        }


        public void email_hash_and_add_to_record(string the_token, int id, string email, string pass)
        {
            // add to login_db database
            string query = "UPDATE login_db SET token = @ptoken WHERE user_id = @p_id;";
            SqlCommand command = new SqlCommand(query, the_database_connection);
            command.Parameters.AddWithValue("@ptoken", the_token);
            command.Parameters.AddWithValue("@p_id", id);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.InsertCommand = command;
            adapter.InsertCommand.ExecuteNonQuery();

            // email to user's email entered from their own email
            MailMessage the_email = new MailMessage(email, email);
            SmtpClient the_smtp_client = new SmtpClient();
            the_smtp_client.Port = 587;
            the_smtp_client.DeliveryMethod = SmtpDeliveryMethod.Network;
            the_smtp_client.UseDefaultCredentials = false;
            the_smtp_client.Credentials = new System.Net.NetworkCredential(email, pass);
            the_smtp_client.EnableSsl = true;
            the_smtp_client.Host = "smtp.gmail.com";
            the_email.Subject = "Password Change - Course Search";
            the_email.Body = "Your token to enter is: " + the_token;
            the_smtp_client.Send(the_email);
            System.Windows.Forms.MessageBox.Show("Email sent");


        }

        public DataTable obtain_matching_token(string user_id)
        {
            string query_matching_token = "SELECT token FROM login_db WHERE user_id = @id";
            SqlCommand matching_token_command = new SqlCommand(query_matching_token, the_database_connection);
            matching_token_command.Parameters.AddWithValue("@id", user_id);
            SqlDataReader matching_id_reader = matching_token_command.ExecuteReader();
            DataTable token_table = new DataTable();
            token_table.Load(matching_id_reader);
            return token_table;
        }
        public void change_password(string user_id, string password)
        {
            string password_change_query = "UPDATE login_db   " +
                                            " SET hashed_password_with_salt = @pwd  " +
                                            "  WHERE user_id = @id ;";
            SqlCommand pass_command = new SqlCommand(password_change_query, the_database_connection);
            pass_command.Parameters.AddWithValue("@pwd", password);
            pass_command.Parameters.AddWithValue("@id", user_id);
            SqlDataAdapter adapter_password = new SqlDataAdapter();
            adapter_password.InsertCommand = pass_command;
            adapter_password.InsertCommand.ExecuteNonQuery();
            System.Windows.Forms.MessageBox.Show("Password changed");
        }
    }
}

