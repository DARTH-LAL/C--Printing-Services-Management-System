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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OopFinalProject
{
    public partial class WorkerTask : Form
    {
        private int userId;
        public WorkerTask(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserRequests();
            LoadNewTasks();
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
            WorkerDash workerDashboard = new WorkerDash(userId);
            workerDashboard.Show();
            this.Hide();
        }

        private void AssignedTaskBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
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

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadUserRequests()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            DataTable dataTable = new DataTable(); // Create a new DataTable to store the results of the SQL query

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Define the SQL query to retrieve service requests assigned to the logged-in user
                    string query = @"
            SELECT s.RequestID, s.ServiceType, s.FeesPerPageRM, s.Quantity, s.TotalPriceRM, s.UrgentRequest, s.Status, 
                   u.UserID, u.Username
            FROM ServiceRequests s
            JOIN allusers u ON s.AssignedWorkerID = u.UserID
            WHERE s.AssignedWorkerID = @UserId"; 

                    using (SqlCommand command = new SqlCommand(query, connection)) // Create a new SqlCommand object to execute the query
                    {
                        command.Parameters.AddWithValue("@UserId", userId); // Add the userId parameter to the SQL query to filter by the logged-in user's ID

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                        dataGridView2.DataSource = dataTable;  // Set the DataGridView's DataSource to the filled DataTable
                        dataGridView2.AutoGenerateColumns = true; 
                        dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        dataGridView2.Columns["UrgentRequest"].ReadOnly = true;  // Set the UrgentRequest column to read-only
                        dataGridView2.Columns["UrgentRequest"].HeaderText = "Urgent Request"; // Set the header text of the UrgentRequest column
                        dataGridView2.Columns["UserID"].HeaderText = "User ID"; // Set the header text of the UserID column
                        dataGridView2.Columns["Username"].HeaderText = "Username"; // Set the header text of the Username column
                    }
                }
                catch (Exception ex)
                { 
                    MessageBox.Show("Failed to load request data: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void New_Task_List_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadNewTasks()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True";
            
            string query = "SELECT RequestID, ServiceType, UrgentRequest FROM ServiceRequests WHERE Status IN ('Assigned', 'WIP') AND AssignedWorkerID = @UserId ORDER BY UrgentRequest DESC";  // Define the SQL query to retrieve tasks that are assigned or in progress for the logged-in user, ordered by urgency
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection); // Create a new SqlCommand object to execute the query
                command.Parameters.AddWithValue("@UserId", userId);   // Add the userId parameter to the SQL query to filter by the logged-in user's ID

                SqlDataAdapter adapter = new SqlDataAdapter(command); // Create a SqlDataAdapter to fill the DataTable with the results of the query
                DataTable tasks = new DataTable("Assigned Tasks"); // Create a new DataTable to store the results of the SQL query
                adapter.Fill(tasks); // Fill the DataTable with the results of the query
                New_Task_List.Items.Clear();       // Clear the existing items in the New_Task_List           

                foreach (DataRow row in tasks.Rows)  // Loop through each row in the DataTable and add a formatted string to the New_Task_List
                {
                    string displayText = $"{row["ServiceType"]} (ID: {row["RequestID"]}) (Urgent: {row["UrgentRequest"]})"; // Create a display text for each task with its service type, ID, and urgency status
                    New_Task_List.Items.Add(displayText); // Add the display text to the New_Task_List
                }
            }
        }

        private void WorkInProgBtn_Click(object sender, EventArgs e)
        {
            UpdateTaskStatus("WIP"); // Call UpdateTaskStatus method with 'WIP' status to update the task status to 'Work In Progress'
        }

        private void CompleteBtn_Click(object sender, EventArgs e)
        {
            UpdateTaskStatus("Completed"); // Call UpdateTaskStatus method with 'Completed' status to update the task status to 'Completed'
        }

        private void UpdateTaskStatus(string newStatus)
        {
            if (New_Task_List.SelectedItem == null) // Check if a task is selected from the list
            {
                MessageBox.Show("Please select a task from the list.", "Update Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Exit the method if no task is selected
            }
            // Extract the task ID from the selected task string
            string selectedTask = New_Task_List.SelectedItem.ToString();
            int taskIdIndex = selectedTask.IndexOf("ID: ") + 4;
            int endIndex = selectedTask.IndexOf(')', taskIdIndex);
            string taskIdString = selectedTask.Substring(taskIdIndex, endIndex - taskIdIndex);
            int taskId = int.Parse(taskIdString);

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string getStatusQuery = "SELECT Status FROM ServiceRequests WHERE RequestID = @RequestID"; // Query to get the current status of the selected task
                SqlCommand getStatusCommand = new SqlCommand(getStatusQuery, connection);
                getStatusCommand.Parameters.AddWithValue("@RequestID", taskId); // Add task ID parameter to the query

                try
                {
                    connection.Open();
                    string currentStatus = getStatusCommand.ExecuteScalar().ToString(); // Execute the query to get the current status

                    if (newStatus == "Completed" && currentStatus != "WIP") // Check if the task can be updated to 'Completed' status based on its current status
                    {
                        MessageBox.Show("A task must be in 'Work In Progress' status before it can be set to 'Completed'.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Exit the method if the current status is not 'WIP'
                    }

                    string updateQuery = "UPDATE ServiceRequests SET Status = @NewStatus WHERE RequestID = @RequestID"; // Query to update the status of the selected task
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@NewStatus", newStatus); // Add new status parameter to the query
                    updateCommand.Parameters.AddWithValue("@RequestID", taskId); // Add task ID parameter to the query

                    int result = updateCommand.ExecuteNonQuery(); // Execute the update query
                    if (result > 0) // Check if the update was successful   
                    {
                        MessageBox.Show("Task status updated to " + newStatus, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadNewTasks();  // Reload the tasks to reflect the status update
                    }
                    else
                    {
                        MessageBox.Show("Failed to update task status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating task status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserRequests();  
            LoadNewTasks();

        }
    }
}
