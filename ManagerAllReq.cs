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
    public partial class ManagerAllReq : Form
    {
        private int userId;
        public ManagerAllReq(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserRequests();
            RefreshData();
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
            ManagerDash managerDashboard = new ManagerDash(userId);
            managerDashboard.Show();
            this.Hide();
        }

        private void AllReqBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
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
        private void LoadUserRequests()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            DataTable dataTable = new DataTable(); // Create a new DataTable to store the fetched data

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // SQL query that fetches request details and user details, joining the ServiceRequests and allusers tables
                    string query = @"
            SELECT s.RequestID, s.ServiceType, s.FeesPerPageRM, s.Quantity, s.TotalPriceRM, s.UrgentRequest, s.Status, 
                   u.UserID, u.Username
            FROM ServiceRequests s
            JOIN allusers u ON s.CustomerID = u.UserID
            ORDER BY s.UrgentRequest DESC, s.RequestID ASC";  

                    using (SqlCommand command = new SqlCommand(query, connection)) // Create a SqlCommand object to execute the query
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command); // SqlDataAdapter handles data fetching
                        adapter.Fill(dataTable); // Fills the DataTable with data returned from the query
                        dataGridView1.DataSource = dataTable;  // Assigns the DataTable as the data source for the DataGridView
                        dataGridView1.AutoGenerateColumns = true;  // Enables automatic generation of columns based on the data source
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sets the column resizing mode to fit the DataGridView's area

                        dataGridView1.Columns["UrgentRequest"].ReadOnly = true;  // Sets the 'UrgentRequest' column to be read-only
                        dataGridView1.Columns["UrgentRequest"].HeaderText = "Urgent Request"; // Renames the 'UrgentRequest' column header for display clarity
                        dataGridView1.Columns["UserID"].HeaderText = "User ID"; // Renames the 'UserID' column header
                        dataGridView1.Columns["Username"].HeaderText = "Username";  // Renames the 'Username' column header
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load request data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void total_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void pending_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void RefreshData()
        {
            int totalRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests");  // Retrieves the total number of service requests from the database
            int progressRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE Status IN ('New', 'Assigned', 'WIP')"); // Retrieves the count of service requests that are new, assigned, or in progress
            int completedRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE Status = 'Completed'"); // Retrieves the count of service requests that have been marked as completed

            UpdateChart(total_req_Chart, totalRequests, "Total Requests"); // Calls UpdateChart method to update the chart for total requests
            UpdateChart(pending_req_Chart, progressRequests, "Requests in Progress"); // Calls UpdateChart method to update the chart for requests in progress
            UpdateChart(completed_req_Chart, completedRequests, "Completed Requests"); // Calls UpdateChart method to update the chart for completed requests
        }

        private void UpdateChart(Chart chart, int count, string seriesName)
        {
            chart.Series.Clear(); // Clears any existing series in the chart

            Series series = chart.Series.Add(seriesName); // Adds a new series with the provided name to the chart
            series.Points.Add(count);   // Adds a data point with the count value to the new series

            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;   // Disables the label style on the X-axis to enhance chart clarity
            chart.ChartAreas[0].AxisY.Minimum = 0;   // Sets the minimum value of the Y-axis to 0 for proper scaling

            series.IsValueShownAsLabel = true;  // Enables displaying values on the chart for each column
        }
        private int GetCount(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(query, connection); // Creates a SQL command by associating the query and connection

                if (parameters != null) // If parameters are provided, they are added to the command's Parameters collection
                {
                    cmd.Parameters.AddRange(parameters);
                }

                connection.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar()); // Executes the command as a scalar query, which is expected to return a single value (the first column of the first row in the result set)
                connection.Close();

                return result; // Returns the integer result of the query
            }
        }
    }
}
