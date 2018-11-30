using System.Collections.Generic;

namespace AnyCompany
{
    public class CustomerOrder
    {
        public Customer Customer { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}