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
    public partial class AdminYearlyRep : Form
    {
        private int userId; // Private field to store the user ID of the logged-in admin
        public AdminYearlyRep(int userId)
        {
            InitializeComponent();
        }

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();  // Closes the current form
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
                else if (selectedType == "Service Report") // Service report form
                {
                    AdminServiceRep serviceReportForm = new AdminServiceRep(userId);
                    serviceReportForm.Show();
                }
                else if (selectedType == "Customer Report") // Customer report form
                {
                    AdminCustomerRep customerReportForm = new AdminCustomerRep(userId);
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void YearcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GenBtn_Click(object sender, EventArgs e)
        {
            if (YearcomboBox.SelectedItem == null) // Check if a year has not been selected from the YearcomboBox
            {
                MessageBox.Show("Please select a year.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Display a message prompting user to select a year
                return; // Exit the method if year is not selected
            }

            string selectedYear = YearcomboBox.SelectedItem.ToString(); // Retrieve the selected year from the YearcomboBox as a string
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True"; // Connection string to the database
            // SQL query to calculate total income grouped by month and filtered by the selected year
            string query = @"
    SELECT 
        MONTH(RequestDate) AS MonthNumber, 
        SUM(TotalPriceRM) AS TotalIncome 
    FROM ServiceRequests
    WHERE YEAR(RequestDate) = @Year
    GROUP BY MONTH(RequestDate)
    ORDER BY MONTH(RequestDate)";

            using (SqlConnection connection = new SqlConnection(connectionString)) // Open a connection to the database using the connection string
            {
                connection.Open(); // Open the database connection
                using (SqlCommand command = new SqlCommand(query, connection)) // Create a SqlCommand object with the SQL query and the connection object
                {
                    command.Parameters.AddWithValue("@Year", selectedYear); // Add parameter for the selected year to the SQL command

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command)) // SqlDataAdapter to execute the command and fill a DataTable
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);  // Fill the DataTable with data
                        if (dataTable.Rows.Count > 0) // Check if the DataTable has any rows returned from the query
                        {
                            dataTable.Columns.Add("Month", typeof(string)); // Add a new column "Month" to display the month name instead of month number
                            foreach (DataRow row in dataTable.Rows)  // Convert each month number to month name and fill the new column
                            {
                                row["Month"] = MonthNameFromNumber(Convert.ToInt32(row["MonthNumber"]));   // Remove the MonthNumber column after its values have been converted
                            }
                            dataTable.Columns.Remove("MonthNumber");  // Remove the MonthNumber column after its values have been converted
                            dataTable.Columns["Month"].SetOrdinal(0);  // Set the Month column to be the first column in the DataTable
                            dataGridView1.DataSource = dataTable; // Set the DataGridView's DataSource to the filled DataTable
                        }
                        else
                        {
                            dataGridView1.DataSource = null;  // Set DataGridView's DataSource to null if no data foun
                            MessageBox.Show("No data found for the selected year.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        // Helper method to convert month number to month name using DateTime formatting
        private string MonthNameFromNumber(int monthNumber)
        {
            DateTime date = new DateTime(1, monthNumber, 1); // Create a DateTime object with the first day of the given month number
            return date.ToString("MMMM"); // Return the full month name
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
                if (saveFileDialog.ShowDialog() == DialogResult.OK) // Display the save file dialog and check if the user clicked OK
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png); // Save the bitmap image to the specified file path in PNG format
                    MessageBox.Show("Export Successful!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); // Notify the user that the export was successful
                }
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
