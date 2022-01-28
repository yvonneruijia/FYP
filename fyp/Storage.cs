using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace fyp
{
    internal class Storage
    {
        public Storage (int x, int y, int z)
        {
            X = x; Y = y; Z = z;
            rack = new StorageLine[x, y, z];
            //Debug.Assert(rack[1, 1, 1] == null);
        }
        internal StorageLine[,,] rack { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int capacity { get; set; }

        public bool assignStorage(InboundShipment shipment)
        {

            foreach (var line in shipment.inboundShipmentLines)
            {
                if (capacity >= X * Y * Z) return false;

                int x = 0, y = 0, z = 0;

                while (rack[x, y, z] != null)
                {
                    x = new Random().Next(0, X);
                    y = new Random().Next(0, Y);
                    z = new Random().Next(0, Z);
                }

                var storageLine = new StorageLine(line.sku, line.arrivalQty, -1);
                rack[x, y, z] = storageLine;
                capacity += 1;
            }

            return true;

        }

    }

    internal class StorageLine
    {
        public StorageLine(SKU _sku, int _qty, int _weight)
        {
            sku = _sku;
            qty = _qty;
            weight = _weight;
        }

        public SKU sku { get; set; }
        public int qty { get; set; }
        public int weight { get; set; }

    }

}
