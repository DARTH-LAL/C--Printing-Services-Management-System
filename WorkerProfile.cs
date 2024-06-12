using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OopFinalProject
{
    public partial class WorkerProfile : Form
    {
        private int userId;
        public WorkerProfile(int userId)
        {
            InitializeComponent();
            InitializeDataGridView();
            this.userId = userId;
            LoadUserData();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true; // Set the DataGridView to automatically generate columns based on the DataSource 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Set the column width to automatically adjust so that the width of the columns
            dataGridView1.ReadOnly = true;  // Set the grid to be read-only to prevent direct editing of the contents by the user
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
            WorkerTask workerTaskForm = new WorkerTask(userId);
            workerTaskForm.Show();
            this.Hide();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void worker_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void worker_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void worker_number_TextChanged(object sender, EventArgs e)
        {

        }

        private void worker_fullname_TextChanged(object sender, EventArgs e)
        {

        }
        private bool UpdateUserData(int userId, string username, string password, string fullName, string phoneNumber)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // SQL query to update user data in the allusers table
                string updateQuery = @"
            UPDATE allusers
            SET Username = @Username, Password = @Password, FullName = @FullName, PhoneNumber = @PhoneNumber
            WHERE UserID = @UserId";

                using (SqlCommand command = new SqlCommand(updateQuery, connection)) // Create a command object to execute the query, associating it with the current database connection
                {
                    // Add parameters to the command to protect against SQL injection and provide values to the SQL query
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@UserId", userId);

                    int result = command.ExecuteNonQuery(); // Execute the query, which returns the number of rows affected
                    return result > 0;  // Check if any rows were affected
                }
            }
        }

        private void LoadUserData()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserID, Role, Username,Password, FullName, PhoneNumber, Registration_Date FROM allusers WHERE UserID = @UserId"; // SQL query to fetch user details from the allusers table for the specified UserID
                using (SqlCommand command = new SqlCommand(query, connection)) // Create a command object to execute the query, associating it with the current database connection
                {
                    command.Parameters.AddWithValue("@UserId", userId);  // Add parameters to the command to protect against SQL injection

                    DataTable dataTable = new DataTable(); // A DataTable to hold the data retrieved from the database
                    SqlDataAdapter adapter = new SqlDataAdapter(command); // Create an adapter to execute the command and fill the DataTable
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0) // Check if the DataTable contains any rows
                    {
                        dataGridView1.DataSource = dataTable;  // If rows are found, bind them to the dataGridView control to displa
                    }
                    else
                    {
                        MessageBox.Show("No user data found for the specified ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(worker_username.Text)) // Check if the username text box is empty or contains only whitespace
            {
                MessageBox.Show("Username cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsUsernameUnique(worker_username.Text, userId)) // Check if the username is unique by calling the IsUsernameUnique 
            {
                MessageBox.Show("Username is already in use. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(worker_password.Text) || worker_password.Text.Length < 4 ||
                !worker_password.Text.Any(char.IsDigit) || !worker_password.Text.Any(ch => !char.IsLetterOrDigit(ch))) // Validate password to ensure it is at least 4 characters long, contains at least one digit, and one non-alphanumeric character
            {
                MessageBox.Show("Password must be at least 4 characters long and include at least one number and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(worker_fullname.Text)) // Check if the full name text box is empty or contains only whitespace
            {
                MessageBox.Show("Full name cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (worker_fullname.Text.Trim().Length <= 4) // Ensure that full name is more than 4 characters long
            {
                MessageBox.Show("Full name must be more than 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(worker_number.Text) || worker_number.Text.Length != 10 || !worker_number.Text.All(char.IsDigit)) // Validate phone number to ensure it is exactly 10 digits long and contains only numbers
            {
                MessageBox.Show("Phone number must be exactly 10 digits long and contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true; // Return true if all validations pass
        }
        public bool IsUsernameUnique(string username, int? userId = null)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (var connect = new SqlConnection(connectionString))
            {
                connect.Open();
                // SQL query that checks if a username exists in the database
                string query = userId.HasValue ?
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username AND UserID != @UserID" :
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username";

                using (var cmd = new SqlCommand(query, connect)) // Prepares the SQL command and assigns the parameters for the query
                {
                    cmd.Parameters.AddWithValue("@Username", username); // Parameter for the username
                    if (userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId.Value); // Parameter for the user ID
                    }
                    int count = (int)cmd.ExecuteScalar(); // Executes the query and returns the count of records found
                    return count == 0; // Returns true if no records are found, indicating the username is unique
                }
            }
        }


        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) // Calls the ValidateInputs method to ensure all user inputs are valid
                return;

            // Retrieves values from the form controls
            string username = worker_username.Text;
            string password = worker_password.Text;
            string fullName = worker_fullname.Text;
            string phoneNumber = worker_number.Text;

            if (UpdateUserData(userId, username, password, fullName, phoneNumber)) // Calls the UpdateUserData method to attempt to update the user's data in the database
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to update profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearBtn_Click(object sender, EventArgs e)
        {
            worker_username.Clear(); // Clears the text from the worker_username textbox
            worker_password.Clear(); // Clears the text from the worker_password textbox
            worker_fullname.Clear(); // Clears the text from the worker_fullname textbox
            worker_number.Clear();  // Clears the text from the worker_number textbox
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)  // Checks if the clicked row index is valid
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Retrieves the DataGridViewRow object representing the clicked row

                worker_username.Text = Convert.ToString(row.Cells["Username"].Value);  // Sets the text of the worker_username textbox to the value from the "Username" cell
                worker_password.Text = Convert.ToString(row.Cells["Password"].Value);  // Sets the text of the worker_password textbox to the value from the "Password" cell
                worker_fullname.Text = Convert.ToString(row.Cells["FullName"].Value); // Sets the text of the worker_fullname textbox to the value from the "FullName" cell
                worker_number.Text = Convert.ToString(row.Cells["PhoneNumber"].Value);  // Sets the text of the worker_number textbox to the value from the "PhoneNumber" cell

            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }
    }
}
