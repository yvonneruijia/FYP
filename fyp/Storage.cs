using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp
{
    public class Storage
    {
        internal StorageLine StorageLine { get; set; }
        public int StorageWeight { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int AvailableState { get; set; }
    }

    internal class StorageLine
    {
        public Storage Storage { get; set; }
        public List<SKU> SKU { get; set; }
        public int QtyAtLocation { get; set; }
        public int NewQtyAtLocation { get; set; }
    }

}
