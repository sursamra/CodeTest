using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCompany
{
   public class CustomerService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IOrderRepository orderRepository;

        public CustomerService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
        }

        public IEnumerable<CustomerOrder> LoadCustomerWithLinkedOrders()
        {
            var customerOrders = new List<CustomerOrder>();
            var customers = customerRepository.GetAllCustomers();

            foreach (Customer  customer in customers)
            {
                var orders = orderRepository.GetCustomerOrders(customer);
                customerOrders.Add(new CustomerOrder() { Customer = customer, Orders = orders });
            }

            return customerOrders;
        }
    }
}
