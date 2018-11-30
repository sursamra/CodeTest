using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AnyCompany
{
    public static class CustomerRepository
    {
        private static string ConnectionString = @"Data Source=(local);Database=Customers;User Id=admin;Password=password;";

        public static Customer Load(int customerId)
        {
            Customer customer = new Customer();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {

                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT Name,Country,DateOfBirth FROM Customer WHERE CustomerId = " + customerId,
                        connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        customer.ID = Convert.ToInt16(reader["ID"]);
                        customer.Name = reader["Name"].ToString();
                        customer.DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString());
                        customer.Country = reader["Country"].ToString();
                    }

                    connection.Close();
                    return customer;
                }
            }
            catch (SqlException sqlexp)
            {
                throw new ApplicationException("SQL error while reteriving customers ", sqlexp);
            }
            catch (Exception exp)
            {
                throw new ApplicationException("Unknown error while reteriving customers ", exp);
            }
        }

        public static IEnumerable<Customer> GetAllCustomers()
        {
            List<Customer> customersList = new List<Customer>();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT DISTINCT C.Name, C.ID CustomerId, C.Country FROM Customer C JOIN Order O ON O.CustomerId = C.Id Order by Name", connection);

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var customer = new Customer();
                        customer.ID = Convert.ToInt32(reader["CustomerId"]);
                        customer.Name = reader["Name"].ToString();
                        customer.Country = reader["Country"].ToString();

                        customersList.Add(customer);
                    }
                }

                return customersList;
            }
            catch (SqlException sqlexp)
            {
                throw new ApplicationException("SQL error while reteriving customers ", sqlexp);
            }
            catch (Exception exp)
            {
                throw new ApplicationException("Unknown error while reteriving customers ", exp);
            }
        }
    }
}


