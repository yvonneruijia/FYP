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
        public Storage(int x, int y, int z)
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

                // Inefficient, we could maintain a list of empty rack locations
                // and select in O1 time
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

        public void pickStorage(OutboundOrder order)
        {
            foreach (var line in order.outboundOrderLines)
            {
                // for each shipmentline, iterate through all storage lines to pick sku
                for (int i = 0; i < X * Y * Z; i++)
                {
                    // i = x + y*X + z*(X*Y)
                    int x = i % X;
                    int y = ((i - x) / X) % Y;
                    int z = (i - x - y) / (X * Y);

                    var stline = rack[x, y, z];

                    if (stline == null)
                    {
                        // empty rack, continue
                        continue;
                    }

                    if (line.sku == stline.sku)
                    {
                        // found a rack, pick as many qty as possible
                        int numPicked = Math.Min(line.orderQty, stline.qty);
                        line.orderQty -= numPicked;
                        stline.qty -= numPicked;

                        if (stline.qty == 0)
                        {
                            // no more item on storage line, set it to empty
                            rack[x, y, z] = null;
                        }

                        if (line.orderQty == 0)
                        {
                            // finished picking the current orderline
                            break;
                        }
                    }
                }
                if (line.orderQty != 0)
                {
                    Console.WriteLine("[ERROR] Not enough storage to pick!!");
                    listStorageLines();
                    Environment.Exit(0);
                }
            }
        }

        public void listStorageLines()
        {
            for (int i = 0; i < X * Y * Z; i++)
            {
                // i = x + y*X + z*(X*Y)
                int x = i % X;
                int y = ((i - x) / X) % Y;
                int z = (i - x - y) / (X * Y);
                Console.WriteLine("(x,y,z) = ({0},{1},{2}) ", x, y, z);

                var stline = rack[x, y, z];

                if (stline == null)
                {
                    // empty rack
                    Console.WriteLine("Empty");
                }
                else
                {
                    Console.WriteLine(stline);
                }

            }
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

        public override String ToString()
        {
            return String.Format("SKU {0} Qty {1} Weight{2}: ", sku, qty, weight);
        }

    }

}
