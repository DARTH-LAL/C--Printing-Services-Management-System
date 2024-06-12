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
    public partial class AdminCustomerRep : Form
    {
        private int userId; // Private field to store the user ID of the logged-in admin
        public AdminCustomerRep(int userId)
        {
            InitializeComponent();
            LoadCustomers(); // Calls the method to load customer data into the form
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the current form
        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; // Minimizes the form

        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            Admin adminForm = new Admin(userId); // Creates an instance of the Admin form
            adminForm.Show();  // Shows the Admin form
            this.Hide();  // Hides the current (AdminCustomerRep) form
        }

        private void ReportBtn_Click(object sender, EventArgs e)
        {
            this.Refresh(); // Refreshes the form
        }

        private void UserBtn_Click(object sender, EventArgs e)
        {
            AdminUsers adminUsersForm = new AdminUsers(userId); // Creates an instance of the AdminUsers form
            adminUsersForm.Show();  // Displays the AdminUsers form
            this.Hide(); // Hides the current form
        }

        private void SignOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);  // Prompt the user to confirm they want to sign out with a dialog box
            if (dialogResult == DialogResult.Yes)
            {

                Form1 loginForm = new Form1(); // Creates an instance of the login form
                loginForm.Show(); // Shows the login form
                this.Close(); // Closes the current form
            }
        }

        private void RepTypecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RepTypecomboBox.SelectedItem != null) // Checks if an item is selected in the combo box
            {
                string selectedType = RepTypecomboBox.SelectedItem.ToString(); // Retrieves the selected item as a string

                this.Hide();  // Hides the current form

                if (selectedType == "Yearly Report")
                {
                    AdminYearlyRep yearlyReportForm = new AdminYearlyRep(userId); // Yearly report form
                    yearlyReportForm.Show();
                }
                else if (selectedType == "Service Report")
                {
                    AdminServiceRep serviceReportForm = new AdminServiceRep(userId); // Service report form
                    serviceReportForm.Show();
                }
                else if (selectedType == "Customer Report")
                {
                    AdminCustomerRep customerReportForm = new AdminCustomerRep(userId);  // Customer report form
                    customerReportForm.Show();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void MonthcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void YearcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GenBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null || YearcomboBox.SelectedItem == null || MonthcomboBox.SelectedItem == null)  // Check if all the required fields are selected in the form
            {
                MessageBox.Show("Please select a customer, year, and month.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method if any selection is missing
            }

            string selectedCustomerItem = listBox1.SelectedItem.ToString(); // Extracting the customer ID from the selected item in the list box
            int idStart = selectedCustomerItem.IndexOf("ID: ") + 4; // Finds the start of the ID
            int idEnd = selectedCustomerItem.IndexOf(")", idStart);  // Finds the end of the ID
            int customerId = int.Parse(selectedCustomerItem.Substring(idStart, idEnd - idStart));  // Parses the ID

            int selectedYear = int.Parse(YearcomboBox.SelectedItem.ToString());  // Extracting the year and month from the respective combo boxes
            string selectedMonthName = MonthcomboBox.SelectedItem.ToString();

            int selectedMonth = MonthNameToNumber(selectedMonthName); // Converting month name to number using a helper method

            if (selectedMonth == -1) // Checking if the month name is valid
            {
                MessageBox.Show("Invalid month name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True";  // Setup SQL connection and command for querying total requests and total income
            string query = @"
        SELECT COUNT(*) AS TotalRequests, SUM(TotalPriceRM) AS TotalIncome
        FROM ServiceRequests
        WHERE CustomerID = @CustomerId AND YEAR(RequestDate) = @Year AND MONTH(RequestDate) = @Month";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId); // Adds the customer ID as a parameter to the SQL command to filter results by the selected customer
                command.Parameters.AddWithValue("@Year", selectedYear);  // Adds the selected year as a parameter to the SQL command to filter results by the specified year
                command.Parameters.AddWithValue("@Month", selectedMonth); // Adds the selected month as a parameter to the SQL command to filter results by the specified month


                DataTable dataTable = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataTable); 
                dataTable.Columns.Add("Month", typeof(string));  // Add and populate "Month" column at the start of the DataTable
                dataTable.Columns["Month"].SetOrdinal(0); // Sets the first position for the column
                foreach (DataRow row in dataTable.Rows)
                {
                    row["Month"] = selectedMonthName; // Set the name of the month for each row
                }
                dataGridView1.DataSource = dataTable;  // Display the query results in dataGridView

            }
        }

        private int MonthNameToNumber(string monthName) // Method to convert the name of a month to its corresponding numerical value
        {
            var monthNames = new Dictionary<string, int>
    {
        {"January", 1}, {"February", 2}, {"March", 3},
        {"April", 4}, {"May", 5}, {"June", 6},
        {"July", 7}, {"August", 8}, {"September", 9},
        {"October", 10}, {"November", 11}, {"December", 12}
    };

            if (monthNames.TryGetValue(monthName, out int monthNumber))
            {
                return monthNumber;  // Returns the month number if found
            }
            return -1;  // Returns - 1 if the month name is not found, indicating an error
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) // Check if the DataGridView contains any rows before attempting to export
            {
                MessageBox.Show("No data to export.", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Show a message box indicating that there is no data to export
                return; // Exit the method if there is no data
            }
            Bitmap bitmap = new Bitmap(dataGridView1.Width, dataGridView1.Height); // Create a bitmap image with the same dimensions as the DataGridView
            dataGridView1.DrawToBitmap(bitmap, new Rectangle(0, 0, dataGridView1.Width, dataGridView1.Height)); // Draw the content of the DataGridView onto the bitmap

            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) // Open a Save File Dialog to choose where to save the image
            {
                saveFileDialog.Filter = "PNG Files|*.png|JPEG Files|*.jpg"; // Set the filter to only show PNG and JPEG options
                saveFileDialog.DefaultExt = "png"; // Set the default file extension to PNG
                if (saveFileDialog.ShowDialog() == DialogResult.OK)  // Display the save file dialog and check if the user clicked OK
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);  // Save the bitmap image to the specified file path in PNG format
                    MessageBox.Show("Export Successful!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); // Notify the user that the export was successful
                }
            }
        }
        private void LoadCustomers()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; // Define the connection string to the database
            string query = "SELECT UserID, Username FROM allusers WHERE Role = 'Customer'";  // Define the SQL query to select UserID and Username from allusers table where the user's role is 'Customer'
            using (SqlConnection connection = new SqlConnection(connectionString)) // Establish a connection to the database
            using (SqlCommand command = new SqlCommand(query, connection)) // Create a command object with the SQL query and the connection object
            using (SqlDataAdapter adapter = new SqlDataAdapter(command)) // Create a data adapter to retrieve data using the command object
            {
                DataTable customers = new DataTable(); // Create a DataTable to hold the retrieved data
                adapter.Fill(customers); // Fill the DataTable with data from the database
                listBox1.Items.Clear(); // Clear existing items from the listBox to avoid duplicate entries
                foreach (DataRow row in customers.Rows) // Iterate through each row in the DataTable
                {
                    string displayText = $"{row["Username"]} (ID: {row["UserID"]})"; // Format the display text as 'Username (ID: UserID)'
                    listBox1.Items.Add(displayText); // Add the formatted text to the listBox
                }
            }
        }

        private void AdminCustomerRep_Load(object sender, EventArgs e)
        {

        }
    }
}
