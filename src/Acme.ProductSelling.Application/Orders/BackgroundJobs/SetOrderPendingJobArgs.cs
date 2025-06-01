using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Orders.BackgroundJobs
{
    [Serializable]
    public class SetOrderPendingJobArgs
    {
        public Guid OrderId { get; set; }
    }
}
