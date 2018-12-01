using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyCompany;

namespace AnyCompanyUnitTests
{
    [TestClass]
    public class OrderServiceTest
    {
        [TestMethod]
        public void TestPlaceAnOrderLinkedToCustomer()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var orderService = new OrderService(orderRepository.Object, customerRepository.Object);
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

            // Let create an order that is not placed yet
            Order order = new Order()
            {
                Amount = 500
            };
            var orders = new List<Order>()
            {
              order
            };

            // lets order it for Tina
            Customer c = customers.Find(x => x.Name == "Tina");    
            
            orderRepository.Setup(r => r.Save(order)).Callback(()=>order.Customer = c);
            customerRepository.Setup(r => r.Load(c.ID)).Returns(c);
                       
            var success = orderService.PlaceOrder(order, 2);

            orderRepository.Setup(r => r.GetCustomerOrders(c)).Returns(orders);
            customerRepository.Setup(r => r.GetAllCustomers()).Returns(customers);

            var customersWithOrders = customerService.LoadCustomerWithLinkedOrders();

            //Verify no order returned for John.           
            Assert.IsTrue(customersWithOrders.First(x => x.Customer.Name == "John").Orders.Count() == 0);

            //Verify order returned belongs to Tina.
            Assert.IsTrue(customersWithOrders.First(x => x.Customer.Name == "Tina").Orders.Count() == 1);

            Assert.IsTrue(success);
          
        }

        [TestMethod]
        public void TestNonUKVAT()
        {

            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var orderService = new OrderService(orderRepository.Object, customerRepository.Object);
            var customerService = new CustomerService(customerRepository.Object, orderRepository.Object);

            var customers = new List<Customer>
            {
                new Customer()
                {
                    ID = 1,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-50),Name = "John"
                },
                new Customer()
                {
                    ID = 2,Country = "GE",DateOfBirth = DateTime.Now.AddYears(-40),Name = "Tina"
                }
            };

            // Let create an order that is not placed yet
            Order order = new Order()
            {
                Amount = 500
            };
            var orders = new List<Order>()
            {
              order
            };

            // lets order it for Tina
            Customer c = customers.Find(x => x.Name == "Tina");

            orderRepository.Setup(r => r.Save(order)).Callback(() => order.Customer = c);
            customerRepository.Setup(r => r.Load(c.ID)).Returns(c);

            var success = orderService.PlaceOrder(order, 2);

            orderRepository.Setup(r => r.GetCustomerOrders(c)).Returns(orders);
            customerRepository.Setup(r => r.GetAllCustomers()).Returns(customers);

            var customersWithOrders = customerService.LoadCustomerWithLinkedOrders();

            Assert.IsTrue(success);
            //Verify order returned belongs to Tina.
            Assert.IsTrue(customersWithOrders.First(x => x.Customer.Name == "Tina").Orders.Count() == 1
                            && customersWithOrders.First(x => x.Customer.Name == "Tina").Orders.First().VAT ==0 );

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestUKVAT()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var orderService = new OrderService(orderRepository.Object, customerRepository.Object);
            var customerService = new CustomerService(customerRepository.Object, orderRepository.Object);
            
            var customers = new List<Customer>
            {
                new Customer()
                {
                    ID = 1,Country = "UK",DateOfBirth = DateTime.Now.AddYears(-50),Name = "John"
                },
                new Customer()
                {
                    ID = 2,Country = "GE",DateOfBirth = DateTime.Now.AddYears(-40),Name = "Tina"
                }
            };

            // Let create an order that is not placed yet
            Order order = new Order()
            {
                Amount = 500
            };
            var orders = new List<Order>()
            {
              order
            };

            // lets order it for John
            Customer c = customers.Find(x => x.Name == "John");

            orderRepository.Setup(r => r.Save(order)).Callback(() => order.Customer = c);
            customerRepository.Setup(r => r.Load(c.ID)).Returns(c);

            var success = orderService.PlaceOrder(order, 1);

            orderRepository.Setup(r => r.GetCustomerOrders(c)).Returns(orders);
            customerRepository.Setup(r => r.GetAllCustomers()).Returns(customers);

            var customersWithOrders = customerService.LoadCustomerWithLinkedOrders();

            Assert.IsTrue(success);
            //Verify order returned belongs to John.
            Assert.IsTrue(customersWithOrders.First(x => x.Customer.Name == "John").Orders.Count() == 1
                            && customersWithOrders.First(x => x.Customer.Name == "John").Orders.First().VAT == 0.2);                     

        }
        [TestMethod]
        public void TestPlaceAnInvalidOrderLinkedToCustomer()
        {
            var orderRepository = new Mock<IOrderRepository>();
            var customerRepository = new Mock<ICustomerRepository>();

            var orderService = new OrderService(orderRepository.Object, customerRepository.Object);

            var order = new Order() { Amount = 0 };
            var customer = new Customer() { ID = 1, Name = "John", Country = "UK" };

            orderRepository.Setup(r => r.Save(It.IsAny<Order>()));
            bool saved = false;
            orderRepository.Setup(r => r.Save(order)).Callback(() => saved = true);
            var success = orderService.PlaceOrder(order, 1);

            Assert.IsFalse(success);

            //Verify placing an order never called save method on Order repository
            Assert.IsFalse(saved);
        }
    }
}
