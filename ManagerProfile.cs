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
    public partial class ManagerProfile : Form
    {
        private int userId;
        public ManagerProfile(int userId)
        {
            InitializeComponent();
            InitializeDataGridView();
            this.userId = userId;
            LoadUserData();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;   // Sets the DataGridView to automatically generate columns based on the data source
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Adjusts the columns to fill the width of the DataGridView, ensuring that all space is used efficiently
            dataGridView1.ReadOnly = true;   // Sets the DataGridView to be read-only, preventing users from modifying the cell values 
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
            ManagerAssignTask managerAssignTaskForm = new ManagerAssignTask(userId);
            managerAssignTaskForm.Show();
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

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) // Checks if the input validation fails. If it does, it exits the method to prevent further execution
                return;
            // Retrieves the username, password, full name, and phone number from the form's text fields
            string username = manager_username.Text;
            string password = manager_password.Text;
            string fullName = manager_fullname.Text;
            string phoneNumber = manager_number.Text;

            if (UpdateUserData(userId, username, password, fullName, phoneNumber)) // Attempts to update the manager's data in the database. If successful, it shows a success message
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to update profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // If the update fails, it shows an error message
            }
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            manager_username.Clear(); // Clears the text from the manager's username, password, full name, and phone number input fields
            manager_password.Clear();
            manager_fullname.Clear();
            manager_number.Clear();
        }

        private void manager_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void manager_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void manager_number_TextChanged(object sender, EventArgs e)
        {

        }

        private void manager_fullname_TextChanged(object sender, EventArgs e)
        {

        }
        private bool UpdateUserData(int userId, string username, string password, string fullName, string phoneNumber)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // SQL query that updates user details
                string updateQuery = @"
            UPDATE allusers
            SET Username = @Username, Password = @Password, FullName = @FullName, PhoneNumber = @PhoneNumber
            WHERE UserID = @UserId";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username); // Adds parameters to the SQL command to protect against SQL injection
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@UserId", userId);

                    int result = command.ExecuteNonQuery(); // Executes the update operation and returns the number of affected rows
                    return result > 0; // Returns true if one or more rows were affected, indicating a successful update
                }
            }
        }

        private void LoadUserData()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // SQL query to fetch user details by user ID
                string query = "SELECT UserID, Role, Username,Password, FullName, PhoneNumber, Registration_Date FROM allusers WHERE UserID = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // Adds the user ID parameter to the SQL command to fetch the correct user

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable); // Fills the DataTable with data fetched from the database

                    if (dataTable.Rows.Count > 0)  // Checks if data is present in the DataTable
                    {
                        dataGridView1.DataSource = dataTable;  // Sets the DataGridView source to the DataTable
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
            if (string.IsNullOrWhiteSpace(manager_username.Text)) // Checks if the username text field is empty
            {
                MessageBox.Show("Username cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsUsernameUnique(manager_username.Text, userId)) // Calls the IsUsernameUnique method to check if the username is already in use, except for the current user
            {
                MessageBox.Show("Username is already in use. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(manager_fullname.Text))  // Checks if the full name text field is empty
            {
                MessageBox.Show("Full name cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (manager_fullname.Text.Trim().Length <= 4) // Checks if the length of the trimmed full name is less than or equal to 4 characters
            {
                MessageBox.Show("Full name must be more than 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(manager_password.Text) || manager_password.Text.Length < 4 ||
                !manager_password.Text.Any(char.IsDigit) || !manager_password.Text.Any(ch => !char.IsLetterOrDigit(ch))) // Checks if the password is less than 4 characters, does not contain a digit, or does not contain a special character
            {
                MessageBox.Show("Password must be at least 4 characters long and include at least one number and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(manager_number.Text) || manager_number.Text.Length != 10 || !manager_number.Text.All(char.IsDigit)) // Checks if the phone number is not exactly 10 digits or contains non-numeric characters
            {
                MessageBox.Show("Phone number must be exactly 10 digits long and contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public bool IsUsernameUnique(string username, int? userId = null)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (var connect = new SqlConnection(connectionString))
            {
                connect.Open();
                // SQL query that checks the existence of the username. If a userID is provided, it excludes that user from the check
                string query = userId.HasValue ?
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username AND UserID != @UserID" :
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username";

                using (var cmd = new SqlCommand(query, connect))// Executes the query
                {
                    cmd.Parameters.AddWithValue("@Username", username); // Adds the username parameter to the SQL command
                    if (userId.HasValue) // Adds the userId parameter to the SQL command if it is not null
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId.Value);
                    }
                    int count = (int)cmd.ExecuteScalar(); // Executes the command and converts the result to an integer, which represents the count of records found
                    return count == 0; // Returns true if no records are found (username is unique), otherwise false
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)  // Checks if the clicked row index is valid
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Retrieves the row that was clicked using the row index


                manager_username.Text = Convert.ToString(row.Cells["Username"].Value); // Sets the manager_username textbox to the Username value from the clicked row
                manager_password.Text = Convert.ToString(row.Cells["Password"].Value);  // Sets the manager_password textbox to the Password value from the clicked row
                manager_fullname.Text = Convert.ToString(row.Cells["FullName"].Value); // Sets the manager_fullname textbox to the FullName value from the clicked row
                manager_number.Text = Convert.ToString(row.Cells["PhoneNumber"].Value);  // Sets the manager_number textbox to the PhoneNumber value from the clicked row

            }
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }
    }
}
