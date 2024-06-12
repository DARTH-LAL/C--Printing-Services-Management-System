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
    public partial class CustomerProfile : Form
    {
        private int userId;

        public CustomerProfile(int userId)
        {
            InitializeComponent();
            InitializeDataGridView();
            this.userId = userId;
            LoadUserData();
            
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AutoGenerateColumns = true;  // Set the AutoGenerateColumns property to true to automatically create columns for each field in the data source
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Set the AutoSizeColumnsMode to Fill so that columns will resize to fill the DataGridView completely, ensuring all available space is used
            dataGridView1.ReadOnly = true;  // Set the ReadOnly property to true to prevent the user from editing the data directly in the DataGridView
        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
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
            CustomerReqList reqListForm = new CustomerReqList(userId);
            reqListForm.Show();
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void customer_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void customer_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void customer_number_TextChanged(object sender, EventArgs e)
        {

        }

        private void customer_fullname_TextChanged(object sender, EventArgs e)
        {

        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) // First, validate input fields using the ValidateInputs method; if validation fails, exit the method early
                return;
            // Retrieve the values from the user input fields
            string username = customer_username.Text;
            string password = customer_password.Text;
            string fullName = customer_fullname.Text;
            string phoneNumber = customer_number.Text;

            if (UpdateUserData(userId, username, password, fullName, phoneNumber))// Attempt to update the user data in the database
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); // If the update is successful, display a success message
            }
            else
            {
                MessageBox.Show("Failed to update profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // If the update fails, display an error message
            }
        }


        private bool UpdateUserData(int userId, string username, string password, string fullName, string phoneNumber)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();// Open the database connection
                // SQL query to update user data in the database
                string updateQuery = @"
            UPDATE allusers
            SET Username = @Username, Password = @Password, FullName = @FullName, PhoneNumber = @PhoneNumber
            WHERE UserID = @UserId";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    // Add parameters to the command to prevent SQL injection
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@UserId", userId);

                    int result = command.ExecuteNonQuery();  // Execute the command and return true if at least one row was updated
                    return result > 0;
                }
            }
        }

        private void LoadUserData()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserID, Role, Username,Password, FullName, PhoneNumber, Registration_Date FROM allusers WHERE UserID = @UserId"; // SQL query to fetch user data based on the given UserID
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // Attach the UserID parameter to the command to filter the data

                    DataTable dataTable = new DataTable();  // Create a DataTable to hold the query results
                    SqlDataAdapter adapter = new SqlDataAdapter(command);  // Execute the command and fill the dataTable with data
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0) // Check if the dataTable contains any rows of data
                    { 
                        dataGridView1.DataSource = dataTable;  // If data exists, set it as the source for the dataGridView
                    }
                    else
                    {
                        MessageBox.Show("No user data found for the specified ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); // If no data is found, display a message informing the user
                    }
                }
            }
        }


        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(customer_username.Text)) // Check if the username text field is empty
            {
                MessageBox.Show("Username cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsUsernameUnique(customer_username.Text, userId)) // Check if the entered username is unique
            {
                MessageBox.Show("Username is already in use. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer_fullname.Text)) // Ensure the full name field is not empty
            {
                MessageBox.Show("Full name cannot be left blank.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (customer_fullname.Text.Trim().Length <= 4) // Ensure the full name is longer than 4 characters
            {
                MessageBox.Show("Full name must be more than 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer_password.Text) || customer_password.Text.Length < 4 ||
                !customer_password.Text.Any(char.IsDigit) || !customer_password.Text.Any(ch => !char.IsLetterOrDigit(ch))) // Validate the password's length and content
            {
                MessageBox.Show("Password must be at least 4 characters long and include at least one number and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(customer_number.Text) || customer_number.Text.Length != 10 || !customer_number.Text.All(char.IsDigit)) // Validate the phone number's length and content
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
                // Determine the SQL query based on whether a userId is provided. It checks for username uniqueness,ignoring the current user's ID if provided (useful during updates)
                string query = userId.HasValue ?
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username AND UserID != @UserID" :
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username";

                using (var cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@Username", username); // Add the username to the command parameters to prevent SQL injection
                    if (userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId.Value);  // Add the userID to the command parameters to exclude the current user from the check
                    }
                    int count = (int)cmd.ExecuteScalar(); // Execute the query and return the number of users with this username
                    return count == 0; // Return true if no users exist with the given username, indicating uniqueness
                }
            }
        }



        private void ClearBtn_Click(object sender, EventArgs e)
        {
            customer_username.Clear();// Clears the text from the customer_username textbox, effectively removing any text the user has entered
            customer_password.Clear(); // Clears the text from the customer_password textbox, removing any password the user may have typed
            customer_fullname.Clear(); // Clears the text from the customer_fullname textbox, erasing any full name input previously entered
            customer_number.Clear(); // Clears the text from the customer_number textbox, deleting any phone number details previously input

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Check if the row index from the event args is valid to avoid errors related to header row clicks or out-of-range indexes
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Retrieve the entire row that the user clicked on using the row index provided by the event arguments

                customer_username.Text = Convert.ToString(row.Cells["Username"].Value);  // Extract the username from the selected row's 'Username' cell and convert it to string before setting it to the customer_username textbox
                customer_password.Text = Convert.ToString(row.Cells["Password"].Value);  // Extract the password from the selected row's 'Password' cell, convert it to string, and assign it to the customer_password textbox
                customer_fullname.Text = Convert.ToString(row.Cells["FullName"].Value);  // Extract the full name from the 'FullName' cell of the selected row, convert it to string, and populate the customer_fullname textbox
                customer_number.Text = Convert.ToString(row.Cells["PhoneNumber"].Value); // Extract the phone number from the 'PhoneNumber' cell of the selected row, convert it to string, and fill the customer_number textbox

            }

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }
    }
}
