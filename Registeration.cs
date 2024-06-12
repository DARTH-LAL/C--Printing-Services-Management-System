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
    public partial class Registeration : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True");
        public Registeration()
        {
            InitializeComponent();
            signup_password.PasswordChar = '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void signup_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void signup_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            signup_password.PasswordChar = checkBox1.Checked ? '\0' : '*';  // If checkBox1 is checked, set the PasswordChar of the signup_password textbox to '\0' (null character), making the password visible.
                                                                            // If checkBox1 is unchecked, set the PasswordToken of the signup_password textbox to '*', hiding the password.
            signup_password.Refresh(); // Forces the signup_password textbox to redraw itself to reflect the change in the visibility of the password characters
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void signup_fullname_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void signup_number_TextChanged(object sender, EventArgs e)
        {

        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(signup_username.Text.Trim()) // Checks if any of the required fields are empty. Trims whitespace for accurate validation
            || string.IsNullOrWhiteSpace(signup_password.Text.Trim())
            || string.IsNullOrWhiteSpace(signup_fullname.Text.Trim())
            || string.IsNullOrWhiteSpace(signup_number.Text.Trim()))
            {
                MessageBox.Show("Please fill all blank fields"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (signup_username.Text.Trim().Length < 4)  // Validates that the username is at least 4 characters long
            {
                MessageBox.Show("Username must be at least 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!IsValidPassword(signup_password.Text.Trim())) // Validates the password complexity  
            {
                MessageBox.Show("Password must be at least 4 characters long, include at least one number, and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (signup_fullname.Text.Trim().Length <= 4)   // Validates the full name length.
            {
                MessageBox.Show("Full name must be more than 4 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!IsValidPhoneNumber(signup_number.Text.Trim()))   // Validates the phone number format
            {
                MessageBox.Show("Phone number must be exactly 10 digits long and contain only numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string role_var = "Customer"; // Default role is set to "Customer"
                if (!string.IsNullOrWhiteSpace(Role_Key.Text.Trim())) // Checks if a role key is provided and not empty, then assigns the appropriate role based on the key
                {
                    switch (Role_Key.Text.Trim())
                    {
                        case "A911":
                            role_var = "admin";
                            break;
                        case "M911":
                            role_var = "manager";
                            break;
                        case "W911":
                            role_var = "worker";
                            break;
                        default:
                            MessageBox.Show("Invalid role key entered , please try again or leave empty for Customer Registration ", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                    }
                }



                    if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        string selectUsername = "SELECT COUNT(UserId) FROM allusers WHERE username = @user"; // SQL command to check if the username is already taken

                        using (SqlCommand checkUser = new SqlCommand(selectUsername, connect))
                        {
                            checkUser.Parameters.AddWithValue("@user", signup_username.Text.Trim());
                            int count = (int)checkUser.ExecuteScalar();

                            if (count >= 1) // If the username is taken, informs the user
                            {
                                MessageBox.Show(signup_username.Text.Trim() + " username is already taken"
                                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                DateTime today = DateTime.Today; // Inserts new user data into the database

                                string insertData = "INSERT INTO allusers" +
                                    "(Username,Password,Registration_Date,FullName,PhoneNumber, Role)" +
                                    "VALUES(@Username,@Password,@Registration_Date,@FullName,@PhoneNumber, @Role)";

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@Username", signup_username.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Password", signup_password.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Registration_Date", today);
                                    cmd.Parameters.AddWithValue("@FullName", signup_fullname.Text.Trim());
                                    cmd.Parameters.AddWithValue("@PhoneNumber", signup_number.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Role", role_var);


                                    cmd.ExecuteNonQuery();

                                    // Confirms successful registration and redirects to the login form
                                    MessageBox.Show("Registered Successfully!"
                                       , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    Form1 loginForm = new Form1();
                                    loginForm.Show();
                                    this.Hide();
                                }
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    finally
                    {
                        connect.Close();
                    }
                }
            }

        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 4) return false;  // Check if the password length is less than 4 characters. If true, return false

            bool containsNumber = password.Any(char.IsDigit); // Check if the password contains at least one digit
            bool containsSpecial = password.Any(ch => !char.IsLetterOrDigit(ch)); // Check if the password contains at least one special character (any character that is not a letter or digit)

            return containsNumber && containsSpecial; // Return true if the password contains both a number and a special character
        }
        private bool IsValidPhoneNumber(string number)
        {
            return number.Length == 10 && number.All(char.IsDigit); // Check if the phone number is exactly 10 characters long and all characters are digits
        }
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 LoginForm = new Form1();
            LoginForm.Show();
            this.Hide();
        }

        private void Registeration_Load(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Role_Key_TextChanged(object sender, EventArgs e)
        {

        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
