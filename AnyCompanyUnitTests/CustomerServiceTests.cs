using System;
using System.Collections.Generic;
using AnyCompany;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Linq;

namespace AnyCompanyUnitTests
{
    [TestClass]
    public class CustomerServiceTests
    {
        [TestMethod]
        public void TestCustomersWithLinkedOrdersService()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var customerService = new CustomerService(customerRepository.Object, orderRepository.Object);

            var customers = new List<Customer>
            {
                new Customer()
                {
                    ID = 1,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-50),Name = "John"
                },
                new Customer()
                {
                    ID = 2,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-40),Name = "Tina"
                }
            };
            //setup order for john
            var orders = new List<Order>()
            {
                new Order()
                {
                    Amount = 500, Customer = customers.Find(x=>x.Name=="John")
                }
            };
            Customer c = customers.Find(x => x.Name == "John");
            orderRepository.Setup(r => r.GetCustomerOrders(c)).Returns(orders);
            customerRepository.Setup(r => r.GetAllCustomers()).Returns(customers);

            var customersWithOrders = customerService.LoadCustomerWithLinkedOrders();
            
            //Verify order returned belongs to john.
            Assert.IsTrue(customersWithOrders.First(x=>x.Customer.Name == "John").Orders.Count() == 1 );

            //Verify no order returned for Tina.
            Assert.IsTrue(customersWithOrders.First(x => x.Customer.Name == "Tina").Orders.Count() == 0);
        }

        [TestMethod]
        public void TestCustomersWithUnlinkedOrdersService()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var customerService = new CustomerService(customerRepository.Object, orderRepository.Object);

            var customers = new List<Customer>
            {
                new Customer()
                {
                    ID = 1,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-50),Name = "John"
                },
                new Customer()
                {
                    ID = 2,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-40),Name = "Tina"
                }
            };

            var orders = new List<Order> { };

            orderRepository.Setup(r => r.GetCustomerOrders(It.IsAny<Customer>())).Returns(orders);
            customerRepository.Setup(r => r.GetAllCustomers()).Returns(customers);

            var customersWithOrders = customerService.LoadCustomerWithLinkedOrders();

            // Verify none of customer palced any order.
            Assert.IsTrue(customersWithOrders.All(x=>x.Orders.Count() == 0));
        }
    }
}