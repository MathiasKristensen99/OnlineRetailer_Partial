using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class OrderCreatedMessage
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
    }
}
