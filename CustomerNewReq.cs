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
using System.Windows.Forms.DataVisualization.Charting;

namespace OopFinalProject
{
    public partial class CustomerNewReq : Form
    {
        private int userId;
        private List<PrintingService> services = new List<PrintingService>(); // Class-level private field: services to hold a list of available printing services
        public CustomerNewReq(int userId)
        {
            InitializeComponent();
            InitializeServices(); // Calls method to populate the list of services
            InitializeDataGridView(); // Calls method to setup the data grid view settings
            this.userId = userId;
        }

        private void InitializeServices() // Method to initialize and add predefined printing and binding services to the services list
        {
            services.Add(new PrintingService(1, "Printing - Black and White", "A4", 0.80M, 100, 0.10M)); // Adds a black and white printing service to the services list
            services.Add(new PrintingService(2, "Printing - Color", "A4", 2.50M, 100, 0.10M)); // Adds a color printing service to the services list
            services.Add(new PrintingService(3, "Binding - Comb Binding", "n/a", 5.50M, 0, 0M));  // Adds a comb binding service to the services list
            services.Add(new PrintingService(4, "Binding - Thick Cover", "n/a", 9.30M, 0, 0M)); // Adds a thick cover binding service to the services list
            services.Add(new PrintingService(5, "Printing - Poster", "A0", 6.00M, 100, 0.10M)); // Adds various poster printing services for different poster sizes to the services list
            services.Add(new PrintingService(6, "Printing - Poster", "A1", 6.00M, 100, 0.10M));
            services.Add(new PrintingService(7, "Printing - Poster", "A2", 3.00M, 100, 0.10M));
            services.Add(new PrintingService(8, "Printing - Poster", "A3", 3.00M, 100, 0.10M));
        }
        private void CustomerNewReq_Load(object sender, EventArgs e)
        {

        }

      
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void service_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            size.Items.Clear();  // Clears the 'size' ComboBox to ensure it only contains relevant items for the newly selected service type

            if (service_type.SelectedItem != null)  // Checks if an item is selected in the 'service_type' ComboBox
            {
                string selectedServiceType = service_type.SelectedItem.ToString();  // Retrieves the string value of the selected item in 'service_type' ComboBox
                var relevantSizes = services.Where(s => s.ServiceType.Contains(selectedServiceType)) // Queries the 'services' list for sizes related to the selected service type, ensuring no duplicate sizes are added
                                            .Select(s => s.Size)
                                            .Distinct();

                foreach (var sizeOption in relevantSizes) // Iterates over each distinct size and adds it to the 'size' ComboBox
                {
                    size.Items.Add(sizeOption);
                }

                if (size.Items.Count > 0) // Sets the selected index of the 'size' ComboBox to the first item if items exist
                {
                    size.SelectedIndex = 0; 
                }

                UpdateFeesAndDiscounts();  // Calls the method to update the fees and discounts based on the current selections
            }
        }

        private void UpdateFeesAndDiscounts()
        {
            if (service_type.SelectedItem != null && size.SelectedItem != null) // Ensures that an item is selected in both 'service_type' and 'size' ComboBoxes
            {
                var selectedService = services.FirstOrDefault(s => s.ServiceType == service_type.SelectedItem.ToString() && s.Size == size.SelectedItem.ToString()); // Retrieves the first service matching the selected type and size from the 'services' list
                if (selectedService != null) // If a matching service is found, updates the fee per page displayed and recalculates the total price
                {
                    fee_perpage.Text = selectedService.FeesPerUnit.ToString("0.00"); // Formats the fee per unit to two decimal places
                    CalculateAndDisplayTotalPrice(); // Calls the method to calculate and display the total price based on the quantity and discounts
                }
            }
        }

            private void label4_Click(object sender, EventArgs e)
        {

        }

        private void size_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFeesAndDiscounts();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void quantity_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(quantity.Text)) // Check if the 'quantity' TextBox is not empty
            {
                if (int.TryParse(quantity.Text, out int qty) && qty > 1000 && qty < 1) // Try to parse the text to an integer and check if the quantity is out of the allowed range (1-1000)
                {
                    MessageBox.Show("The quantity cannot exceed 1000.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Display an error message if the quantity is not within the valid range
                    quantity.Text = ""; // Clear the 'quantity' TextBox
                }
            }
            CalculateAndDisplayTotalPrice(); // Call a method to calculate and display the total price based on the current inputs

        }


        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void fee_perpage_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void total_price_TextChanged(object sender, EventArgs e)
        {

        }

        private void urgent_request_CheckedChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayTotalPrice();
        }

        private void CalculateAndDisplayTotalPrice()
        {
            if (service_type.SelectedItem != null && int.TryParse(quantity.Text, out int qty))  // Ensure that a service type is selected and the quantity is a valid integer
            {
                var selectedService = services.FirstOrDefault(s => s.ServiceType == service_type.SelectedItem.ToString());  // Retrieve the service details based on the selected service type
                bool isUrgent = urgent_request.Checked; // Check if the 'urgent' checkbox is checked
                if (selectedService != null) // If a valid service is found
                {
                    decimal totalPrice = selectedService.CalculateTotalCost(qty, isUrgent); // Calculate the total cost using the service's pricing model, quantity, and urgency status
                    total_price.Text = totalPrice.ToString("0.00"); // Display the formatted total price in the 'total_price' text box
                }
            }
        }
        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (service_type.SelectedItem == null || string.IsNullOrWhiteSpace(service_type.SelectedItem.ToString())) // Validate that a service type has been selected
            {
                MessageBox.Show("Please select a service type.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;  // Stop execution if no service type is selected
            }
            if (size.SelectedItem == null || string.IsNullOrWhiteSpace(size.SelectedItem.ToString())) // Validate that a size has been selected
            {
                MessageBox.Show("Please select a size.", "Selection Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;  // Stop execution if no size is selected
            }
            if (int.TryParse(quantity.Text, out int qty) && qty <= 1000 && qty > 0)  // Validate that the quantity entered is a valid integer within the specified range
            {
                // Retrieve the selected service type, size, per-page fee, total price, and urgency status
                string serviceType = service_type.SelectedItem?.ToString() ?? "N/A";
                string size = this.size.SelectedItem?.ToString() ?? "N/A";
                string feesPerPage = fee_perpage.Text;
                string totalPrice = total_price.Text;
                string urgent = urgent_request.Checked ? "Yes" : "No";
                // Add the gathered details as a new row in the 'dataGridView1'
                dataGridView1.Rows.Add(serviceType, size, qty, feesPerPage, totalPrice, urgent);
            }
            else
            {
                // Show an error message if the quantity input is invalid
                MessageBox.Show("Please enter a valid quantity (1-1000).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                quantity.Text = "";  // Clear the quantity input field
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        } 
        private void InitializeDataGridView()  
        {
            dataGridView1.Columns.Add("ServiceType", "Service Type"); // Adds a column for the service type with the header "Service Type"
            dataGridView1.Columns.Add("Size", "Size"); // Adds a column for the size of the product or service with the header "Size"
            dataGridView1.Columns.Add("Quantity", "Quantity"); // Adds a column for the quantity with the header "Quantity"
            dataGridView1.Columns.Add("FeesPerPage", "Fees Per Page (RM)"); // Adds a column for the fees per page with the header "Fees Per Page (RM)"
            dataGridView1.Columns.Add("TotalPrice", "Total Price (RM)"); // Adds a column for the total price with the header "Total Price (RM)"
            dataGridView1.Columns.Add("Urgent", "Urgent Request"); // Adds a column to indicate if the request is urgent with the header "Urgent Request"

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sets the column sizing mode to fill, ensuring that all space in the DataGridView is used

            dataGridView1.ReadOnly = true; // Sets the DataGridView to read-only to prevent direct editing of its content
        }
        private void ClearBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();  // Clears all the rows in the DataGridView
            service_type.SelectedIndex = -1; // Resets the selected index for the service type ComboBox to no selection
            size.SelectedIndex = -1;  // Resets the selected index for the size ComboBox to no selection
            quantity.Clear();  // Clears the text in the quantity TextBox
            fee_perpage.Clear(); // Clears the text in the fees per page TextBox
            total_price.Clear(); // Clears the text in the total price TextBox
            urgent_request.Checked = false; // Unchecks the urgent request CheckBox, setting it to false
        } 

        private void SubReqBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].IsNewRow)) // Check if the DataGridView is empty or only contains the template row for new entries
            {
                MessageBox.Show("There are no requests to submit.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Displays a message if no requests are available to submit
            }

            var confirmResult = MessageBox.Show("Are you sure you want to submit the request/s?", "Confirm Submission", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // Confirmation dialog to ensure the user wants to proceed with submitting the requests

            if (confirmResult != DialogResult.Yes) // Check user's response to the confirmation dialog. If not "Yes", cancel submission
            {
                MessageBox.Show("Submission canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // Exit the event handler if submission is canceled
            }
            else
            {
               
                    SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;"); 
                connect.Open();

                try
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows) // Iterate over each row in the DataGridView
                    {
                        if (!row.IsNewRow) // Check that the current row is not the template row
                        {
                            // Extract values from the DataGridView row
                            string serviceType = row.Cells["ServiceType"].Value.ToString();
                            string size = row.Cells["Size"].Value.ToString();
                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal feesPerPage = Convert.ToDecimal(row.Cells["FeesPerPage"].Value);
                            decimal totalPrice = Convert.ToDecimal(row.Cells["TotalPrice"].Value);
                            bool urgent = row.Cells["Urgent"].Value.ToString().Equals("Yes");
                            string status = "New";  // Set the default status for new requests
                            int assignedWorkerID = 0;  // Default worker ID is 0, indicating no worker assigned yet
                            // SQL query to insert the new request into the database
                            string query = "INSERT INTO ServiceRequests (ServiceType, FeesPerPageRM, Quantity, CustomerID, TotalPriceRM, UrgentRequest, Status, AssignedWorkerID) VALUES (@ServiceType, @FeesPerPage, @Quantity, @CustomerID, @TotalPrice, @UrgentRequest, @Status, @AssignedWorkerID)";

                            using (SqlCommand cmd = new SqlCommand(query, connect))
                            {
                                // Add parameters to the SQL command to prevent SQL injection
                                cmd.Parameters.AddWithValue("@ServiceType", serviceType);
                                cmd.Parameters.AddWithValue("@FeesPerPage", feesPerPage);
                                cmd.Parameters.AddWithValue("@Quantity", quantity);
                                cmd.Parameters.AddWithValue("@CustomerID", this.userId);
                                cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                                cmd.Parameters.AddWithValue("@UrgentRequest", urgent);
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@AssignedWorkerID", assignedWorkerID);
                                // Execute the SQL command
                                cmd.ExecuteNonQuery(); 
                            }
                        }
                    }

                    MessageBox.Show("Request/s submitted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); // Notify the user of successful submission
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to submit requests", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // Handle exceptions by displaying an error message
                }
                finally
                {
                    connect.Close(); // Ensure the database connection is closed
                }
                dataGridView1.Rows.Clear(); // Clear the DataGridView after submission
            }
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

        private void CrossBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void NewReqBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            CustomerProfile ProfileForm = new CustomerProfile(userId);
            ProfileForm.Show();
            this.Hide();
        }

        private void DashBtn_Click(object sender, EventArgs e)
        {
            Customer_Dash_ CustomerDash = new Customer_Dash_(userId);
            CustomerDash.Show();
            this.Close();
        }

        private void ReqStatusBtn_Click(object sender, EventArgs e)
        {
            CustomerReqList reqListForm = new CustomerReqList(userId);
            reqListForm.Show();
            this.Hide();
        }
    }
}
