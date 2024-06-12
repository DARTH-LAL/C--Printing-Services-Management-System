using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace OopFinalProject
{
    class UserData
    {
        public int Id { get; set; } // Unique identifier for the user
        public string Role { get; set; } // Role of the user in the system (e.g., admin, worker)
        public string Username { get; set; } // Username for the user's login credentials
        public string Password { get; set; } // User's password (stored encrypted)
        public DateTime? RegistrationDate { get; set; }  // Date when the user account was registered
        public string FullName { get; set; } // Full name of the user
        public string PhoneNumber { get; set; } // Contact number of the user

        public string Status { get; set; } // Current status of the user (e.g., active, inactive)

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";

        public List<UserData> GetUserListData() // Method to fetch all user data from the database and return it as a list of UserData objects
        {
            List<UserData> listData = new List<UserData>();

            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT * FROM allusers";  // SQL query to select all records from the 'allusers' table

                    using (SqlCommand command = new SqlCommand(selectData, connect))
                    using (SqlDataReader reader = command.ExecuteReader()) // Executing the command and using SqlDataReader to read the results
                    {
                        while (reader.Read()) // Reading each row from the result set
                        {
                            UserData userData = new UserData() // Creating a new UserData object for each row and initializing its properties
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString(reader.GetOrdinal("Role")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? null : reader.GetString(reader.GetOrdinal("Password")),
                                RegistrationDate = reader.IsDBNull(reader.GetOrdinal("Registration_Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Registration_Date")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Status = reader["Status"].ToString()
                            };
                            listData.Add(userData); // Adding the newly created UserData object to the list
                        }
                    }
                }
                catch (Exception ex) // Catching any exceptions that occur during database operations
                {
                    Console.WriteLine("Error fetching user data: " + ex.Message);
                }
            }
            return listData; // Returning the list of UserData objects containing all user records
        }

        

    }
   
}
