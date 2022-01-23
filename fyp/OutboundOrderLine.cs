using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace fyp
{
    internal class OutboundOrderLine
    {
        public OutboundOrder OutboundOrder { get; set; }
        public List<SKU> SKU { get; set; }
        public int OutboundQty { get; set; }
    }
}
