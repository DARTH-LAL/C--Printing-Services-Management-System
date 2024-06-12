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
    public partial class AdminServiceRep : Form
    {
        private int userId; // Private field to store the user ID of the logged-in admin
        public AdminServiceRep(int userId)
        {
            InitializeComponent();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; // Minimizes the form
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
                    AdminCustomerRep customerReportForm = new AdminCustomerRep(userId); // Customer report form
                    customerReportForm.Show();
                }
            }
        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            Admin adminForm = new Admin(userId); // Creates an instance of the Admin form
            adminForm.Show(); // Shows the Admin form
            this.Hide(); // Hides the current form
        }

        private void ReportBtn_Click(object sender, EventArgs e)
        { 
            this.Refresh(); // Refreshes the form
        }

        private void UserBtn_Click(object sender, EventArgs e)
        {
            AdminUsers adminUsersForm = new AdminUsers(userId); // Creates an instance of the AdminUsers form
            adminUsersForm.Show(); // Displays the AdminUsers form
            this.Hide(); // Hides the current form
        }

        private void SignOutBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to sign out?", "Confirm Sign Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // Prompt the user to confirm they want to sign out with a dialog box
            if (dialogResult == DialogResult.Yes)
            {

                Form1 loginForm = new Form1(); // Creates an instance of the login form
                loginForm.Show(); // Shows the login form
                this.Close(); // Closes the current form
            }
        }

        private void YearcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GenBtn_Click(object sender, EventArgs e)
        {
            if (YearcomboBox.SelectedItem == null) // Check if a year has not been selected from the YearcomboBox
            {
                MessageBox.Show("Please select a year.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);  // Display a message prompting user to select a year
                return;  // Exit the method if year is not selected
            }

            if (MonthcomboBox.SelectedItem == null) // Check if a month has not been selected from the MonthcomboBox
            {
                MessageBox.Show("Please select a month.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Display a message prompting user to select a month
                return;  // Exit the method if month is not selected
            }
            int selectedYear = int.Parse(YearcomboBox.SelectedItem.ToString()); // Parse the selected year from the YearcomboBox
            string selectedMonthName = MonthcomboBox.SelectedItem.ToString(); // Retrieve the selected month name from the MonthcomboBox
            int selectedMonth = MonthNameToNumber(selectedMonthName); // Convert the selected month name to its corresponding number using a helper method

            // SQL query to calculate total requests and total income filtered by year and month
            string query = @"
    SELECT 
        COUNT(*) AS TotalRequests, 
        SUM(TotalPriceRM) AS TotalIncome 
    FROM ServiceRequests
    WHERE 
        YEAR(RequestDate) = @Year AND 
        MONTH(RequestDate) = @Month";

            using (SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True")) // Open a connection to the database using a connection string
            {
                using (SqlCommand command = new SqlCommand(query, connection)) // Create a SqlCommand object with the SQL query and the connection object
                {
                    command.Parameters.AddWithValue("@Year", selectedYear); // Add parameters to the SQL command for year and month
                    command.Parameters.AddWithValue("@Month", selectedMonth);

                    DataTable dataTable = new DataTable(); // DataTable to hold the result of the query
                    SqlDataAdapter adapter = new SqlDataAdapter(command); // SqlDataAdapter to execute the command and fill the DataTable
                        connection.Open();  // Open the database connection
                    adapter.Fill(dataTable);  // Fill the DataTable with data
                    if (dataTable.Rows.Count > 0)  // Check if the DataTable has any rows returned from the query
                    {
                        dataTable.Columns.Add("MonthName", typeof(string)).SetOrdinal(0);  // Add a new column "MonthName" at the first position to display the month name instead of month number
                        foreach (DataRow row in dataTable.Rows)  // Set the month name for all rows in the new column
                        {
                            row["MonthName"] = selectedMonthName;  // Set the month name for all rows
                        }
                        dataGridView1.DataSource = dataTable; // Set the DataGridView's DataSource to the filled DataTable
                    }
                }
            }
        }

        private int MonthNameToNumber(string monthName)   // Method to convert the name of a month to its corresponding numerical value
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
                return monthNumber; // Returns the month number if found
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

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())  // Open a Save File Dialog to choose where to save the image
            {
                saveFileDialog.Filter = "PNG Files|*.png|JPEG Files|*.jpg"; // Set the filter to only show PNG and JPEG options
                saveFileDialog.DefaultExt = "png"; // Set the default file extension to PNG
                if (saveFileDialog.ShowDialog() == DialogResult.OK) // Display the save file dialog and check if the user clicked OK
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png); // Save the bitmap image to the specified file path in PNG format
                    MessageBox.Show("Export Successful!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); // Notify the user that the export was successful
                }
            }
        }

        private void MonthcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes the current form
        }
    }
}
