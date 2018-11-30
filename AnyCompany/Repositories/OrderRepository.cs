using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AnyCompany
{
    internal class OrderRepository:IOrderRepository
    {
        private static string ConnectionString = @"Data Source=(local);Database=Orders;User Id=admin;Password=password;";

        public void Save(Order order)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO Orders VALUES (@OrderId, @Amount, @VAT)", conn);

                    command.Parameters.AddWithValue("@OrderId", order.OrderId);
                    command.Parameters.AddWithValue("@Amount", order.Amount);
                    command.Parameters.AddWithValue("@VAT", order.VAT);

                    command.ExecuteNonQuery();
                }
                catch (SqlException sqlexp)
                {
                    throw new ApplicationException("SQL error while reteriving customers orders ", sqlexp);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException("Unknown error while reteriving customers orders ", exp);
                }
            }
        }
        public IEnumerable<Order> GetCustomerOrders(Customer customer)
        {
            List<Order> orderList = new List<Order>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand("SELECT OrderId, Amount, VAT FROM Order WHERE CustomerId = @CustomerId", conn);
                    command.Parameters.AddWithValue("@CustomerId", customer.ID);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Order order = new Order();
                        order.OrderId = Convert.ToInt32(reader["OrderId"]);
                        order.Amount = Convert.ToDouble(reader["Amount"]);
                        order.VAT = Convert.ToDouble(reader["VAT"]);
                        order.Customer = customer;
                        orderList.Add(order);
                    }
                }
                catch (SqlException sqlexp)
                {
                    throw new ApplicationException("SQL error while reteriving customers orders ", sqlexp);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException("Unknown error while reteriving customers orders ", exp);
                }
            }

            return orderList;
        }
    }
}
