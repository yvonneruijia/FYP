using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;

namespace fyp
{
    internal class Storage
    {
        public Storage(int x, int y, int z, int xa, int ya, int xb, int yb, int xc, int yc)
        {
            X = x; Y = y; Z = z; XA = xa; XB = xb; XC = xc; YA = ya; YB = yb; YC = yc; 
            rack = new StorageLine[x, y, z];
            //Debug.Assert(rack[1, 1, 1] == null);
        }
        internal StorageLine[,,] rack { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int XA { get; set; }
        public int YA { get; set; }
        public int XB { get; set; }
        public int YB { get; set; }
        public int XC { get; set; }
        public int YC { get; set; }

        public int capacity { get; set; }


        public (int, int, int, int) getRangeByPopularity(int popularity)
        {
            int x_l = 0, x_h = 0, y_l = 0, y_h = 0;
            if (popularity == 1)
            {
                // Find in (0,0) -> (XA,YA)
                x_l = 0; x_h = XA; y_l = 0; y_h = YA;

            }
            else if (popularity == 2)
            {
                // Find in (XA,YA) -> (XB,YB)
                x_l = XA; y_l = YA; x_h = XB; y_h = YB;

            }
            else if (popularity == 3)
            {
                // Find in (XB,YB) -> (XC,YC)
                x_l = XB; y_l = YB; x_h = XC; y_h = YC;
            }
            else
            {
                Debug.Assert(false);
            }
            return (x_l, y_l, x_h, y_h);
        }

        public bool assignShipmentLineToZone(InboundShipmentLine line, int x_l, int y_l, int x_h, int y_h)
        {
            // First traverse the X direction
            for (int x = x_l; x < x_h; x++)
            {
                for (int y = 0; y < y_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] == null)
                        {
                            //Console.Write("{0},{1},{2}\n", x, y, z);
                            var storageLine = new StorageLine(line.sku, line.arrivalQty, -1);
                            rack[x, y, z] = storageLine;
                            capacity ++;
                            return true;
                        }
                    }
                }
            }

            // Traverse the Y direction
            for (int x = 0; x < x_h; x++)
            {
                for (int y = y_l; y < y_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] == null)
                        {
                            //Console.Write("{0},{1},{2}\n", x, y, z);
                            var storageLine = new StorageLine(line.sku, line.arrivalQty, -1);
                            rack[x, y, z] = storageLine;
                            capacity ++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool assignStorage(InboundShipment shipment)
        {
            foreach (var line in shipment.inboundShipmentLines)
            {
                if (capacity >= X * Y * Z) return false;
                
                int popularity = line.popularity;
                bool assigned = false;
                while (!assigned)
                {
                    var (x_l, y_l, x_h, y_h) = getRangeByPopularity(popularity);
                    assigned = assignShipmentLineToZone(line, x_l, y_l, x_h, y_h);
                    if (!assigned)
                    {
                        popularity++; // If there is no space in the current zone, de-promote it
                        Console.Write("[De-Promoting] the following shipmentline to a lower zone {0}...\n", popularity);
                        Console.Write(line + "\n");
                    }
                }
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
                            capacity--;
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
