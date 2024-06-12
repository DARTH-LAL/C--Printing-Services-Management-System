using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OopFinalProject
{
    public partial class Customer_Dash_ : Form
    {
        private int userId;

        public Customer_Dash_(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUsername();
            RefreshData();
            
        }

        private void LoadUsername()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Username FROM allusers WHERE UserID = @UserId"; // Defines a SQL query to retrieve the Username for a given UserID from the allusers table
                SqlCommand command = new SqlCommand(query, connection); // Initializes a SqlCommand object with the SQL query and the database connection
                command.Parameters.AddWithValue("@UserId", this.userId); // Adds the userID parameter to the SQL command to prevent SQL injection and ensure that the query executes correctly

                connection.Open(); // Opens the database connection to execute the command
                var result = command.ExecuteScalar();  // Executes the command using ExecuteScalar, which is used here because the query is expected to return a single value (the username)
                if (result != null)  // Checks if the result from the database is not null, indicating that a username was found
                {
                    string username = result.ToString(); // Converts the result to a string, which represents the username
                    label2.Text = "Welcome,\n" + username;  // Sets the text of label2 to welcome the user by their username
                }
            }
        }
        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

        private void SignOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {

                Form1 loginForm = new Form1();
                loginForm.Show();
                this.Close();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void NewReqBtn_Click(object sender, EventArgs e)
        {
            CustomerNewReq newReqForm = new CustomerNewReq(userId);
            newReqForm.Show();
            this.Hide();
        }

        private void ReqStatusBtn_Click(object sender, EventArgs e)
        {
            CustomerReqList reqListForm = new CustomerReqList(userId);
            reqListForm.Show();
            this.Hide();

        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            CustomerProfile ProfileForm = new CustomerProfile(userId);
            ProfileForm.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void RefreshData()
        {
            int totalRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId"); // Calculates the total number of service requests for the logged-in customer
            int progressRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId AND Status IN ('New', 'Assigned', 'WIP')"); // Calculates the number of service requests that are in progress for the logged-in customer
            int completedRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId AND Status = @Status", "Completed"); // Calculates the number of completed service requests for the logged-in customer

            label7.Text = totalRequests.ToString(); // Displays the total number of requests on label7 on the form
            label8.Text = progressRequests.ToString(); // Displays the number of in-progress requests on label8 on the form
            label9.Text = completedRequests.ToString(); // Displays the number of completed requests on label9 on the form

            UpdateChart(total_req_Chart, totalRequests, "Total Requests"); // Updates the chart to visualize total requests 
            UpdateChart(req_progress_Chart, progressRequests, "Requests in Progress"); // Updates the chart to visualize requests in progress
            UpdateChart(completed_req_Chart, completedRequests, "Completed Requests"); // Updates the chart to visualize completed requests
        }

        private void UpdateChart(Chart chart, int count, string seriesName)
        {
            chart.Series.Clear();  // Clears all existing series from the chart
            Series series = chart.Series.Add(seriesName); // Creates and adds a new series with the specified name to the chart
            series.Points.Add(count); // Adds a data point representing the count to the series
            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;   // Disables the X-axis label
            chart.ChartAreas[0].AxisY.Minimum = 0;   // Sets the minimum value of the Y-axis to 0 for proper scaling
            series.ChartType = SeriesChartType.Column;   // Sets the chart type to Column for visualizing data as bars
            series.IsValueShownAsLabel = true;  // Enables displaying the values as labels on the bars in the chart
        }

        private int GetCount(string query, string status = null)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(query, connection); // Prepares a SQL command using the provided query and the open connection

                cmd.Parameters.AddWithValue("@UserId", userId); // Adds the userId parameter to the SQL command to filter records for the logged-in user

                if (status != null) // Adds the status parameter if provided, used for filtering records based on the status
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                connection.Open(); // Opens the database connection
                int result = Convert.ToInt32(cmd.ExecuteScalar()); // Executes the command and converts the result to an integer, which is the count of records
                connection.Close(); // Closes the database connection

                return result; // Returns the count result
            }
        }

        private void total_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void req_progress_Chart_Click(object sender, EventArgs e)
        {

        }

        private void completed_req_Chart_Click(object sender, EventArgs e)
        {

        }
    }
}
