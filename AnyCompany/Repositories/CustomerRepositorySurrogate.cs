using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCompany
{
    /// <summary>
    /// Making customer repository injectable
    /// </summary>
    public class CustomerRepositorySurrogate : ICustomerRepository
    {
        public Customer Load(int customerId) { return CustomerRepository.Load(customerId); }
        public IEnumerable<Customer> GetAllCustomers() { return CustomerRepository.GetAllCustomers(); }

    }
}
