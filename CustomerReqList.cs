using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection.Emit;

namespace OopFinalProject
{
    public partial class CustomerReqList : Form
    {
        private int userId;
        public CustomerReqList(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserRequests();
            RefreshData();
        }

        private void LoadUserRequests()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            DataTable dataTable = new DataTable(); // Create a new DataTable to hold the fetched data

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // SQL query to select various fields from the ServiceRequests table where the CustomerID matches the current user
                    string query = @"
                SELECT RequestID, ServiceType, FeesPerPageRM, Quantity, TotalPriceRM, UrgentRequest, Status 
                FROM ServiceRequests 
                WHERE CustomerID = @UserId";
                    // Create a SqlCommand object to execute the query against the database
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId); // Add the userId parameter to the command to filter requests for the specific user

                        SqlDataAdapter adapter = new SqlDataAdapter(command); // Create a SqlDataAdapter to execute the command and fill the DataTable with data
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // Assign the filled DataTable as the DataSource for the dataGridView
                        dataGridView1.AutoGenerateColumns = true; // Set properties to automatically generate columns based on data and adjust their widths to fit the grid
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        // Set the UrgentRequest column to read-only to prevent editing and set a user-friendly header text
                        dataGridView1.Columns["UrgentRequest"].ReadOnly = true; 
                        dataGridView1.Columns["UrgentRequest"].HeaderText = "Urgent Request"; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load request data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Show an error message if there is an issue in fetching data
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

        private void DashBtn_Click(object sender, EventArgs e)
        {
            Customer_Dash_ CustomerDash = new Customer_Dash_(userId);
            CustomerDash.Show();
            this.Close();
        }

        private void NewReqBtn_Click(object sender, EventArgs e)
        {
            CustomerNewReq newReqForm = new CustomerNewReq(userId);
            newReqForm.Show();
            this.Hide();
        }

        private void ReqStatusBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            CustomerProfile ProfileForm = new CustomerProfile(userId);
            ProfileForm.Show();
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                
        }

      

        private void total_req_Chart_Click(object sender, EventArgs e)
        {

        }

        private void req_progress_Chart_Click(object sender, EventArgs e)
        {

        }

        private void completed_req_Chart_Click_1(object sender, EventArgs e)
        {

        }
        private int GetCount(string query, string status = null)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"))
            {
                SqlCommand cmd = new SqlCommand(query, connection); // Creates a command to execute the query

                cmd.Parameters.AddWithValue("@UserId", userId); // Adds the userId parameter to the SQL command to filter results for the specific user

                if (status != null) // Adds the status parameter to the command if provided
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                }

                connection.Open(); // Opens the connection to the database
                int result = Convert.ToInt32(cmd.ExecuteScalar()); // Executes the query and converts the result to an integer. This assumes the query returns a count
                connection.Close();

                return result; // Returns the count result
            }
        }

        private void RefreshData()
        {
            int totalRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId"); // Retrieves the total number of requests for the customer
            int progressRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId AND Status IN ('New', 'Assigned', 'WIP')"); // Retrieves the number of requests in progress for the customer
            int completedRequests = GetCount("SELECT COUNT(*) FROM ServiceRequests WHERE CustomerID = @UserId AND Status = @Status", "Completed"); // Retrieves the number of completed requests for the customer

            UpdateChart(total_req_Chart, totalRequests, "Total Requests"); // Updates the chart for total requests
            UpdateChart(req_progress_Chart, progressRequests, "Requests in Progress"); // Updates the chart for requests in progress
            UpdateChart(completed_req_Chart, completedRequests, "Completed Requests"); // Updates the chart for completed requests
        }

        private void UpdateChart(Chart chart, int count, string seriesName)
        {
            chart.Series.Clear();  // Clears any existing series in the chart
            Series series = chart.Series.Add(seriesName); // Adds a new series with the specified name
            series.Points.Add(count); // Adds a point to the series with the provided count
            chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;   // Disables the X-axis label
            chart.ChartAreas[0].AxisY.Minimum = 0;   // Sets the minimum value of the Y-axis to 0
            series.ChartType = SeriesChartType.Column;  // Sets the chart type to Column
            series.IsValueShownAsLabel = true;  // Enables displaying values as labels on the series
        }

    }
    }
