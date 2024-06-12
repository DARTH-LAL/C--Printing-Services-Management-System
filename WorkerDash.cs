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
    public partial class WorkerDash : Form
    {
        private int userId;
        public WorkerDash(int userId)
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

        private void AssignedTaskBtn_Click(object sender, EventArgs e)
        {
            WorkerTask workerTaskForm = new WorkerTask(userId);
            workerTaskForm.Show();
            this.Hide();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            WorkerProfile profileForm = new WorkerProfile(userId);
            profileForm.Show();
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

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void RefreshData()
        {
            int assignedTasks = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE AssignedWorkerID = @UserId AND Status = 'Assigned'"); // Retrieves the count of assigned tasks for the current user
            int progressTasks = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE AssignedWorkerID = @UserId AND Status IN ('WIP')"); // Retrieves the count of tasks in progress for the current user
            int completedTasks = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE AssignedWorkerID = @UserId AND Status = 'Completed'"); // Retrieves the count of completed tasks for the current user

            label7.Text = assignedTasks.ToString(); // Updates the label7 with the count of assigned tasks
            label8.Text = progressTasks.ToString(); // Updates the label8 with the count of tasks in progress
            label9.Text = completedTasks.ToString(); // Updates the label9 with the count of completed tasks

            UpdateChart(assigned_task_Chart, assignedTasks, "Assigned Tasks"); // Calls UpdateChart to refresh the assigned tasks chart
            UpdateChart(task_in_prog_Chart, progressTasks, "Tasks in Progress"); // Calls UpdateChart to refresh the tasks in progress chart
            UpdateChart(completed_task_Chart, completedTasks, "Completed Tasks"); // Calls UpdateChart to refresh the completed tasks chart
        }

        private void UpdateChart(Chart chart, int count, string seriesName)
        {
            chart.Series.Clear(); // Clears all previous series in the chart to start fresh
            Series series = chart.Series.Add(seriesName);  // Creates a new series with the specified name and adds it to the chart
            series.Points.Add(count); // Adds the count data as a point in the series
            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false; // Disables the label style on the X-axis to make the chart cleaner
            chart.ChartAreas[0].AxisY.Minimum = 0; // Sets the minimum value of the Y-axis to 0 for clarity in presentation
            series.ChartType = SeriesChartType.Column; // Sets the type of the chart to Column for a bar-like visual representation
            series.IsValueShownAsLabel = true; // Enables the display of values on the chart for each column
        }

        private int GetCount(string query, string status = null)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserId", userId); // Adds a parameter to the SQL command for filtering by user ID

                if (status != null) // Checks if a status is provided and adds it as a parameter to the SQL command
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                connection.Open();
                object result = cmd.ExecuteScalar(); // Executes the SQL command and retrieves the first column of the first row in the result set returned by the query
                connection.Close();

                return Convert.ToInt32(result); // Converts the result to an integer and returns it. This count is the number of rows that match the query conditions
            }
        }

        private void assigned_task_Chart_Click(object sender, EventArgs e)
        {

        }

        private void task_in_prog_Chart_Click(object sender, EventArgs e)
        {

        }

        private void completed_task_Chart_Click(object sender, EventArgs e)
        {

        }

        private void LoadUsername()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; // Adjust the connection string as necessary
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Username FROM allusers WHERE UserID = @UserId"; // Define a SQL query to select the username of a user based on their user ID
                SqlCommand command = new SqlCommand(query, connection); // Create a command object to execute the query, associating it with the connection
                command.Parameters.AddWithValue("@UserId", this.userId); // Add the user ID parameter to the command to specify which user's username to retrieve.

                connection.Open();
                var result = command.ExecuteScalar();  // Execute the command and retrieve the first column of the first row in the result set, if any
                if (result != null)
                {
                    string username = result.ToString(); // Convert the result to a string since ExecuteScalar returns an object
                    label2.Text = "Welcome,\n" + username;  // Update the text of label2 to welcome the user, using the retrieved username
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
