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

namespace OopFinalProject
{
    public partial class ManagerAssignTask : Form
    {
        private int userId;
        public ManagerAssignTask(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserRequests();
            LoadNewTasks();
            LoadWorkers();        
        }

        private void label4_Click(object sender, EventArgs e)
        {

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
            ManagerAllReq allReqForm = new ManagerAllReq(userId);
            allReqForm.Show();
            this.Hide();
        }

        private void AssignTaskBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
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
            DataTable dataTable = new DataTable(); // Creates a new DataTable to hold the data that will be retrieved

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // SQL query to select request details and join with user details
                    string query = @"
            SELECT s.RequestID, s.ServiceType, s.FeesPerPageRM, s.Quantity, s.TotalPriceRM, s.UrgentRequest, s.Status, 
                   u.UserID, u.Username
            FROM ServiceRequests s
            JOIN allusers u ON s.CustomerID = u.UserID
            ORDER BY s.UrgentRequest DESC, s.RequestID ASC";  

                    using (SqlCommand command = new SqlCommand(query, connection)) // Creates a SQL command to execute the query using the established connection
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command); // Creates a SqlDataAdapter to execute the command and fill the DataTable with data
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;  // Sets the data source of the DataGridView to the DataTable
                        dataGridView1.AutoGenerateColumns = true;  // Automatically generates columns for the DataGridView based on the data source
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sets the column sizing mode to fill, ensuring that all columns proportionally fill the available space

                        dataGridView1.Columns["UrgentRequest"].ReadOnly = true;  // Sets the 'UrgentRequest' column to read-only, preventing editing
                        dataGridView1.Columns["UrgentRequest"].HeaderText = "Urgent Request"; // Sets a friendly header text for the 'UrgentRequest' column
                        dataGridView1.Columns["UserID"].HeaderText = "User ID"; // Sets a friendly header text for the 'UserID' column
                        dataGridView1.Columns["Username"].HeaderText = "Username"; // Sets a friendly header text for the 'Username' column
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load request data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Displays an error message if there is an issue loading the data
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void NewTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void WorkerList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserRequests();
            LoadNewTasks();
            LoadWorkers();
        }

        private void assignBtn_Click(object sender, EventArgs e)
        {

            if (New_Task_List.SelectedItem == null || Worker_List.SelectedItem == null) // Checks if both a task and a worker are selected, showing a warning message if not
            {
                MessageBox.Show("Please select both a task and a worker.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Retrieves the selected task string, and extracts the task ID from it
                string selectedTask = New_Task_List.SelectedItem.ToString();
                int taskId = int.Parse(selectedTask.Split(new string[] { "(ID: ", ")" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                // Retrieves the selected worker string, and extracts the worker ID from it
                string selectedWorker = Worker_List.SelectedItem.ToString();
                int workerId = int.Parse(selectedWorker.Split(new string[] { "(ID: ", ")" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; // Connection string to access the local SQL Server database 
                string updateQuery = @" UPDATE ServiceRequests SET AssignedWorkerID = @WorkerId, Status = 'Assigned' WHERE RequestID = @RequestId"; // SQL query to update the assigned worker and status of a service request

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    // Adding parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@WorkerId", workerId);
                    command.Parameters.AddWithValue("@RequestId", taskId);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery(); // Executes the update command and returns the number of rows affected
                        if (rowsAffected > 0) // Checks if any rows were updated, and displays a success message if so
                        {
                            MessageBox.Show("Task assigned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadUserRequests();  // Reloads the user requests to reflect changes

                        }
                        else
                        {
                            MessageBox.Show("No changes were made. Please check your selections.", "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);  // Displays a warning if no changes were made
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to assign task: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Catches and displays any errors that occur during the operation
                    }
                }
            }

        }

    private void LoadNewTasks()
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True";
        string query = "SELECT RequestID, ServiceType, UrgentRequest FROM ServiceRequests WHERE Status = 'New' order by UrgentRequest desc";  // SQL query to retrieve new tasks, ordering them by their urgency
        SqlConnection connection = new SqlConnection(connectionString);  // Initialize the connection using the connection string
        SqlCommand command = new SqlCommand(query, connection); // Prepare a command to execute the SQL query
        SqlDataAdapter adapter = new SqlDataAdapter(command); // Adapter to execute the query and store the results
        DataTable tasks = new DataTable("New Tasks"); // Adapter to execute the query and store the results
            adapter.Fill(tasks); // Fill the DataTable with the results from the database query
            New_Task_List.Items.Clear();       // Clear any existing items in the New_Task_List control         
            foreach (DataRow row in tasks.Rows) // Loop through each row in the DataTable
        {
            string displayText = $"{row["ServiceType"]} (ID: {row["RequestID"]}) (Urgent: {row["UrgentRequest"]})";   // Format and prepare the display text for each task
            New_Task_List.Items.Add(displayText);     // Add the formatted text as a new item in the New_Task_List control
        }
    }

        private void LoadWorkers()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True";
            string query = "SELECT UserID, Username FROM allusers WHERE Role = 'Worker'"; // SQL query to retrieve all workers
            using (SqlConnection connection = new SqlConnection(connectionString)) // Using block to manage resources, ensuring proper disposal of connection and command
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable workers = new DataTable();  // DataTable to hold the worker data
                adapter.Fill(workers); // Fill the DataTable with the results from the database query
                Worker_List.Items.Clear();  // Clear any existing items in the Worker_List control
                foreach (DataRow row in workers.Rows) // Loop through each row in the DataTable
                {
                    string displayText = $"{row["Username"]} (ID: {row["UserID"]})"; // Format and prepare the display text for each worker
                    Worker_List.Items.Add(displayText); // Add the formatted text as a new item in the Worker_List control
                }
            }
        }

        private void NewTaskList_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void Worker_List_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
