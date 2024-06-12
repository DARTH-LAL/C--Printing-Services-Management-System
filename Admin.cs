using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OopFinalProject
{
    public partial class Admin : Form
    {
        private int userId; // Private field to store the user ID of the logged-in admin
        public string LoggedInUsername { get; set; } // Public property to get or set the logged-in admin's username
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"); // Connection to the database using a connection string

        public Admin(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Sets the userId field with the provided user ID
            LoadUsername();
            RefreshData(); // Calls the RefreshData method to load data when the form loads
        }

        private void Admin_Load(object sender, EventArgs e)
        {
           
        }

        private void RefreshData()
        {
            int totalUsers = GetCount("SELECT COUNT(UserID) FROM allusers");  // Retrieves the total number of users
            int activeUsers = GetCount("SELECT COUNT(UserID) FROM allusers WHERE status = @Status", "active");  // Retrieves the number of active users
            int inactiveUsers = GetCount("SELECT COUNT(UserID) FROM allusers WHERE status = @Status", "inactive"); // Retrieves the number of inactive users

            label7.Text = totalUsers.ToString();  // Displays the total number of users in label7
            label8.Text = activeUsers.ToString(); // Displays the number of active users in label8
            label9.Text = inactiveUsers.ToString(); // Displays the number of active users in label9

            UpdateChart(Total_Users_Chart, totalUsers); // Updates the chart for total users
            UpdateChart(Active_Users_Chart, activeUsers); // Updates the chart for active users
            UpdateChart(Inctive_Users_Chart, inactiveUsers); // Updates the chart for inactive users
        }

        private void UpdateChart(Chart chart, int count)
        {
            
            chart.Series[0].Points.Clear();  // Clears all points from the first series in the provided chart to ensure it's ready for new data


            chart.Series[0].Points.AddY(count);  // Adds a new point to the first series in the provided chart with the y-value set to the count parameter
                                                 // This effectively displays the count on the chart
        }




        private int GetCount(string query, string status = null)
        {
        if (connect.State != ConnectionState.Open)  // Check if the connection to the database is not already open, if not, open it.
                connect.Open();

        try
        {
            using (SqlCommand cmd = new SqlCommand(query, connect))  // Create a new SqlCommand using the query string and connection.
                {
                if (status != null) // If a status parameter is provided, add it to the command's parameters.
                    {
                    cmd.Parameters.AddWithValue("@Status", status); 
                }

                object result = cmd.ExecuteScalar(); // Execute the query and store the result. ExecuteScalar is used for fetching a single value.
                    return Convert.ToInt32(result); // Convert the result to an integer and return it. This assumes the query will always return a count.
                }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 0;  // If an exception occurs, display it in a message box and return 0 as the count.
            }
        finally
        {
            if (connect.State == ConnectionState.Open) // Ensure that the connection is closed after the operation is done.
                    connect.Close();
        }
    }


    private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            this.Refresh(); // Refresh the current form, possibly to update any dynamic content or status displayed
        }

        private void ReportBtn_Click(object sender, EventArgs e)
        {
            AdminYearlyRep reportForm = new AdminYearlyRep(userId);  // Create and show the yearly report form while passing the current user ID for context
            reportForm.Show();
            this.Hide();  // Hide the current form to focus user interaction on the report form
        }

        private void UserBtn_Click(object sender, EventArgs e)
        {
            AdminUsers adminUsersForm = new AdminUsers(userId); // Create and show the Admin Users management form, also passing the current user ID
            adminUsersForm.Show();
            this.Hide(); // Hide the current admin main form while the user management form is displayed
        }

        private void SignOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);  // Prompt the user to confirm they want to sign out with a dialog box
            if (dialogResult == DialogResult.Yes)
            {
                
                Form1 loginForm = new Form1(); // If the user confirms, display the login form and close the current admin form
                loginForm.Show();  
                this.Close();      
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; // Minimizes the form

        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the current form
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void Total_Users_Chart_Click(object sender, EventArgs e)
        {
            

        }

        private void Active_Users_Chart_Click(object sender, EventArgs e)
        {
           

        }

        private void Inctive_Users_Chart_Click(object sender, EventArgs e)
        {
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadUsername()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; // Define the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))  // Create a connection using the defined string
            {
                string query = "SELECT Username FROM allusers WHERE UserID = @UserId"; // SQL query to fetch the username of the user identified by userId
                SqlCommand command = new SqlCommand(query, connection);  // Create a command object to execute the query against the database
                command.Parameters.AddWithValue("@UserId", this.userId);   // Parameterize the query to prevent SQL injection and ensure type safety

                connection.Open();  // Open a connection to the database 
                var result = command.ExecuteScalar();  // Execute the query and store the result returned in the variable 'result' ,
                                                       // Use ExecuteScalar to fetch a single value
                if (result != null) // Check if the result is not null, implying the user was found
                {
                    string username = result.ToString(); // Convert the result to a string since ExecuteScalar returns an object
                    label2.Text = "Welcome,\n" + username;   // Update the text of a label control to welcome the user by their username
                }
            }
        }
    }
}

