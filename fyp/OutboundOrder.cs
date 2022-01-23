using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp
{
    internal class OutboundOrder
    {
        public List<OutboundOrderLine> OutboundOrderLine { get; set;}
        public float OutboundTime { get; set; }
    }
}