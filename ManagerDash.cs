using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OopFinalProject
{
    public partial class ManagerDash : Form
    {
        private int userId;
        public ManagerDash(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            RefreshData();
            LoadUsername();
        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void AllReqBtn_Click(object sender, EventArgs e)
        {
            ManagerAllReq allReqForm = new ManagerAllReq(userId);
            allReqForm.Show();
            this.Hide();
        }

        private void AssignTaskBtn_Click(object sender, EventArgs e)
        {
            ManagerAssignTask managerAssignTaskForm = new ManagerAssignTask(userId);
            managerAssignTaskForm.Show();
            this.Hide();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            ManagerProfile managerProfileForm = new ManagerProfile(userId); 
            managerProfileForm.Show();
            this.Hide();
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void total_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void pending_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void completed_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void RefreshData()
        {
            int totalRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests");  // Retrieves the total number of service requests from the database
            int progressRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE Status IN ('New', 'Assigned', 'WIP')"); // Retrieves the count of service requests that are either new, assigned, or in work-in-progress status
            int completedRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE Status = 'Completed'"); // Retrieves the count of service requests that have been completed


            label7.Text = totalRequests.ToString(); // Updates the label to display the total number of requests
            label8.Text = progressRequests.ToString(); // Updates the label to display the number of requests in progress
            label9.Text = completedRequests.ToString(); // Updates the label to display the number of completed requests

            UpdateChart(total_req_Chart, totalRequests, "Total Requests"); // Updates the chart to visualize the total number of requests.
            UpdateChart(pending_req_Chart, progressRequests, "Requests in Progress"); // Updates the chart to visualize the number of requests in progress
            UpdateChart(completed_req_Chart, completedRequests, "Completed Requests"); // Updates the chart to visualize the number of completed requests
        }

        private void UpdateChart(Chart chart, int count, string seriesName)
        {
            chart.Series.Clear(); // Clears all previously displayed series from the chart

            Series series = chart.Series.Add(seriesName); // Adds a new series with the specified name to the chart for displaying data
            series.Points.Add(count);  // Adds a data point to the series, representing the count of requests

            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;   // Disables the display of labels on the X-axis
            chart.ChartAreas[0].AxisY.Minimum = 0;   // Sets the minimum value of the Y-axis to 0 for better scale and presentation

            series.IsValueShownAsLabel = true;   // Enables the display of data labels on the chart, showing the actual values
        }
        private int GetCount(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                if (parameters != null) // Adds parameters to the command if any are provided, protecting against SQL injection
                {
                    cmd.Parameters.AddRange(parameters);
                }

                connection.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar()); // Executes the query and converts the result to an integer. The query is expected to return a count
                connection.Close();

                return result; // Returns the integer result of the query
            }
        }

        private void LoadUsername()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Username FROM allusers WHERE UserID = @UserId"; // SQL query to select the username of a user based on their user ID.
                SqlCommand command = new SqlCommand(query, connection); // Creates a SQL command object to execute the query, associating it with the connection
                command.Parameters.AddWithValue("@UserId", this.userId); // Adds the user ID parameter to the SQL command to specify which user's username to retrieve

                connection.Open();
                var result = command.ExecuteScalar();  // Executes the command and retrieves a single value (the username) from the database
                if (result != null) // If a result is found (i.e., the username exists), converts it to a string
                {
                    string username = result.ToString();
                    label2.Text = "Welcome,\n" + username;  // Sets the text of a label to display the username, welcoming the user
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
