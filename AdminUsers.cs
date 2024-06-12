using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OopFinalProject
{
    public partial class AdminUsers : Form
    {
        private int userId; // Private field to store the user ID of the logged-in admin
        public AdminUsers(int userId)
        {
            InitializeComponent();
            LoadUserData(); // Calls the LoadUserData method to populate the form with data
            this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);  // Add a Click event handler to the ClearBtn button, linking it to the ClearBtn_Click method
            this.userId = userId;
        }

        private void UserBtn_Click(object sender, EventArgs e)
        {
            this.Refresh(); // Refresh the form's data
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;  // Minimizes the form
        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the current form
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            Admin adminForm = new Admin(userId); // Creates an instance of the Admin form
            adminForm.Show();   // Shows the Admin form
            this.Close(); // Closes the current (AdminCustomerRep) form
        }

        private void ReportBtn_Click(object sender, EventArgs e)
        {
            AdminYearlyRep reportForm = new AdminYearlyRep(userId); // Create and show the yearly report form while passing the current user ID for context
            reportForm.Show();
            this.Hide();  // Hide the current form to focus user interaction on the report form
        }

        private void SignOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);  // Prompt the user to confirm they want to sign out with a dialog box
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

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void user_id_TextChanged(object sender, EventArgs e)
        {

        }

        private void user_role_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void user_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void user_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void user_fullname_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void user_number_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if(!ValidateInput()) // Validates input fields using the ValidateInput method and exits if validation fails.
                return;

            if (!string.IsNullOrWhiteSpace(user_id.Text) && int.TryParse(user_id.Text, out int userId)) // Checks if the user_id field is not empty and parses it into integer.
            {
                if (DoesUserIdExist(userId)) // Checks if the parsed userId already exists in the database
                {
                    MessageBox.Show("User ID already exists. Please enter a different user ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Shows a message box if the user ID already exists, requesting a different user ID
                    return;
                }
            }

            string username = user_username.Text; // Retrieves the username from the form
            if (!IsUsernameUnique(username)) // Checks if the username is unique
            {
                MessageBox.Show("Username is already in use. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Shows a message box if the username is already in use
                return;
            }
            // Retrieves password, full name, phone number, role, and status from the form
            string password = user_password.Text;
            string fullName = user_fullname.Text;
            string phoneNumber = user_number.Text;
            string role = user_role.SelectedItem.ToString();
            string status = user_status.SelectedItem.ToString();
            DateTime registrationDate = dateTimePicker1.Value;

            bool result = AddUser(username, password, role, fullName, phoneNumber, registrationDate, status); // Calls the AddUser method with the gathered information and stores the result

            if (result) // If the result is true, displays a success message and reloads the user data
            {
                MessageBox.Show("User added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUserData();
            }
            else
            {
                MessageBox.Show("Failed to add user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // If adding the user failed, displays an error message
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(user_username.Text) || user_username.Text.Length < 4) // Checks if username is entered and has at least 4 characters
            {
                MessageBox.Show("Username must be at least 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(user_password.Text) || user_password.Text.Length < 4 || !user_password.Text.Any(char.IsDigit) || !user_password.Text.Any(ch => !char.IsLetterOrDigit(ch))) // Validates password to ensure it is at least 4 characters, contains a digit and a special character
            {
                MessageBox.Show("Password must be at least 4 characters long and include at least one number and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(user_fullname.Text) || user_fullname.Text.Length <= 4) // Checks if the full name is provided and it is more than 4 characters long
            {
                MessageBox.Show("Full name must be more than 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(user_number.Text) || user_number.Text.Length != 10 || !user_number.Text.All(char.IsDigit)) // Validates the phone number to ensure it's exactly 10 digits long and numeric
            {
                MessageBox.Show("Phone number must be exactly 10 digits long and contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (user_role.SelectedItem == null || string.IsNullOrWhiteSpace(user_role.Text)) // Ensures a role is selected from the dropdown
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (user_status.SelectedItem == null || string.IsNullOrWhiteSpace(user_status.Text)) // Ensures a status is selected from the dropdown
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DateTime registrationDate = dateTimePicker1.Value;  
            if (registrationDate > DateTime.Now) // Checks if the registration date is not set in the future
            {
                MessageBox.Show("The registration date cannot be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true; // Returns true if all validations pass
        }

        private bool AddUser(string username, string password, string role, string fullName, string phoneNumber, DateTime registrationDate, string status)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; // Connection string to connect to the SQL Server database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Opens a connection to the database

                    // SQL query to insert a new user into the allusers table
                    string insertQuery = "INSERT INTO allusers (Role, Username, Password, Registration_Date, FullName, PhoneNumber, Status) " +
                                         "VALUES (@Role, @Username, @Password, @RegistrationDate, @FullName, @PhoneNumber, @Status)";

                    
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        // Adds parameters to the SQL command to prevent SQL injection
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@RegistrationDate", registrationDate);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Status", status);

                        // Executes the SQL command and returns the number of rows affected
                        int rowsAffected = cmd.ExecuteNonQuery();

                        
                        return rowsAffected > 0; // Returns true if one or more rows are affected
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Shows an error message if an exception occurs during the database operation
                    return false;
                }
            }
        }


        private void UpdateBtn_Click(object sender, EventArgs e)
        {

            if (!ValidateInput()) // Validates input fields before proceeding with the update
                return;

            if (!int.TryParse(user_id.Text, out int userId)) // Attempts to parse the user ID from the user_id text field
            {
                MessageBox.Show("Please enter a valid user ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DoesUserIdExist(userId)) // Checks if the user ID exists in the database
            {
                MessageBox.Show("User ID does not exist.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsUsernameUnique(user_username.Text, userId)) // Ensures the username is unique except for the current user
            {
                MessageBox.Show("Username is already in use. Please choose a different username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Retrieves values from the form fields
            string username = user_username.Text;
            string password = user_password.Text;
            string fullName = user_fullname.Text;
            string phoneNumber = user_number.Text;
            string role = user_role.SelectedItem.ToString();
            string status = user_status.SelectedItem.ToString();
            DateTime registrationDate = dateTimePicker1.Value;

            bool result = UpdateUser(userId, username, password, role, fullName, phoneNumber, registrationDate, status); // Calls the UpdateUser method and checks the result

            if (result) // Displays a success message if the update was successful
            {
                MessageBox.Show("User updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUserData();  // Reloads the user data to reflect changes
            }
            else
            {
                MessageBox.Show("Failed to update user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Displays an error message if the update failed
            }
        }

        private bool UpdateUser(int userId, string username, string password, string role, string fullName, string phoneNumber, DateTime registrationDate, string status)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; // Connection string for accessing the SQL database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Opens the database connection

                    // SQL command to update user details based on the UserID
                    string updateQuery = "UPDATE allusers SET Username = @Username, Password = @Password, " +
                                         "Role = @Role, FullName = @FullName, PhoneNumber = @PhoneNumber, " +
                                         "Registration_Date = @RegistrationDate, Status = @Status WHERE UserID = @UserID";

                    
                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        // Assigns values to parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@RegistrationDate", registrationDate);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                       
                        int rowsAffected = cmd.ExecuteNonQuery(); // Executes the update command and returns the number of rows affected


                        return rowsAffected > 0; // Returns true if one or more rows were updated
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Displays an error message if the update fails
                    return false;
                }
            } 
        }
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if ((user_id.Text == this.userId.ToString())) // Checks if the user is attempting to delete themselves, which is not allowed
            {
                MessageBox.Show("Unable to delete yourself :(", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(user_id.Text) || !int.TryParse(user_id.Text, out int userId)) // Validates the user ID input field for correctness 
            {
                MessageBox.Show("Inavlid User Id", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           


            // Confirmation dialog to make sure the user really wants to delete the selected user
            var confirmResult = MessageBox.Show("Are you sure you want to delete this user?",
                                                "Confirm Deletion",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes) // If the user confirms deletion, proceed to delete the user
            {
                if (DeleteUser(userId)) // Calls the DeleteUser method and checks if deletion was successful
                {
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUserData(); 
                }
                else
                {
                    MessageBox.Show("Error deleting user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Shows error if there was error while deleteing
                }
            }
        }

        private bool DeleteUser(int userId)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"; // Connection string for accessing the SQL database
            List<UserData> listData = new List<UserData>(); // The listData variable is declared 


            using (var connect = new SqlConnection(connectionString))
            {
                try
                {
                    connect.Open(); // Opens the database connection.
                    string deleteQuery = "DELETE FROM allusers WHERE UserID = @UserID"; // SQL command to delete a user by UserID

                    using (var cmd = new SqlCommand(deleteQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId); // Assigns the user ID to the parameter in the SQL query
                        int result = cmd.ExecuteNonQuery(); // Executes the deletion command and returns the number of affected rows
                        return result > 0;  // Returns true if any row was affected
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting user: " + ex.Message);  // Outputs the error to the console if the deletion fails
                    return false; // Returns false if an exception occurs
                }
            }
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            user_id.Text = string.Empty; // Clears the user ID text box
            user_username.Text = string.Empty; // Clears the username text box
            user_password.Text = string.Empty; // Clears the password text box
            user_fullname.Text = string.Empty; // Clears the full name text box
            user_number.Text = string.Empty; // Clears the phone number text box


            user_role.SelectedIndex = -1; // Resets the user role dropdown to show no selection
            user_status.SelectedIndex = -1;  // Resets the user status dropdown to show no selection


            dateTimePicker1.Value = DateTime.Now; // Resets the date picker to the current date

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void user_status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LoadUserData()
        {
            try
            {
                UserData userData = new UserData();
                List<UserData> users = userData.GetUserListData(); // Fetches a list of all users from the database
                if (users.Any()) // Checks if there are any users returned
                {
                    dataGridView1.DataSource = users; // Sets the DataGridView's data source to the list of users
                }
                else
                {
                    MessageBox.Show("No data available."); // Shows a message if no user data is found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message); // Displays an error message if an exception occurs
            }
        }

        public bool IsUsernameUnique(string username, int? userId = null)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (var connect = new SqlConnection(connectionString))
            {
                connect.Open(); // Opens a connection to the database
                string query = userId == null ?
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username" :
                    "SELECT COUNT(*) FROM allusers WHERE Username = @Username AND UserID != @UserID"; // SQL query to count users with the given username. Excludes a specific user if userId is provided

                using (var cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    if (userId != null) cmd.Parameters.AddWithValue("@UserID", userId.Value); // Adds the user ID to the query if provided
                    int count = (int)cmd.ExecuteScalar(); // Executes the query and returns the count of records
                    return count == 0; // Returns true if no users with the same username exist, indicating the username is unique
                }
            }
        }

        public bool DoesUserIdExist(int userId)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
            using (var connect = new SqlConnection(connectionString))
            {
                connect.Open();  // Opens a connection to the database
                string query = "SELECT COUNT(*) FROM allusers WHERE UserID = @UserID"; // SQL query to count users with the specified user ID

                using (var cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId); // Adds the user ID to the query
                    int count = (int)cmd.ExecuteScalar(); // Executes the query and returns the count of records
                    return count > 0; // Returns true if the user exists, false otherwise
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Check if the clicked row index is valid
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Retrieve the clicked row from the DataGridView based on the index provided by the event arguments

                // Extract data from each cell of the row and update the corresponding form controls
                user_id.Text = row.Cells[0].Value.ToString();  // Update the user ID text box with the value from the first cell
                user_role.Text = row.Cells[1].Value.ToString();  // Update the user role text box with the value from the second cell
                user_username.Text = row.Cells[2].Value.ToString(); // Update the username text box with the value from the third cell
                user_password.Text = row.Cells[3].Value.ToString(); // Update the password text box with the value from the fourth cell
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells[4].Value); // Set the dateTimePicker control to the date from the fifth cell
                user_fullname.Text = row.Cells[5].Value.ToString();  // Update the full name text box with the value from the sixth cell
                user_number.Text = row.Cells[6].Value.ToString();  // Update the phone number text box with the value from the seventh cell
                user_status.Text = row.Cells[7].Value.ToString(); // Update the status text box with the value from the eighth cell
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }
    }
}
