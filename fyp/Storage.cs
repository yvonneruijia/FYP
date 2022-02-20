using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace fyp
{
    internal class Storage
    {
        public Storage(int x, int y, int z, int xa, int ya, int xb, int yb, int xc, int yc)
        {
            X = x; Y = y; Z = z; XA = xa; XB = xb; XC = xc; YA = ya; YB = yb; YC = yc; 
            rack = new StorageLine[x, y, z];
            SKUToPopularity = new Dictionary<int, int>();
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

        public Dictionary<int, int> SKUToPopularity; // sku -> popularity


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

        public int getPopularityByLoc(int x, int y, int z)
        {
            if (x < XA && y < YA) return 1;
            else if (x < XB && y < YB) return 2;
            else if (x < XC && y < YC) return 3;
            else return -1;
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
                            var storageLine = new StorageLine(line.sku, line.arrivalQty, -1, line.popularity);
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
                            var storageLine = new StorageLine(line.sku, line.arrivalQty, -1, line.popularity);
                            rack[x, y, z] = storageLine;
                            capacity ++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void relocate(SKU sku, int fromPopularity, int toPopularity)
        {
            // find an empty slot in target popularity zone
            var (xt_l, yt_l, xt_h, yt_h) = getRangeByPopularity(toPopularity);
            var (xf_l, yf_l, xf_h, yf_h) = getRangeByPopularity(fromPopularity);

            // First traverse the X direction
            for (int x = xt_l; x < xt_h; x++)
            {
                for (int y = 0; y < yt_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] == null)
                        {
                            // found empty target slot
                            var line = getRelocatedStorageLine(sku, xf_l, xf_h, yf_l, yf_h);
                            if (line == null) return; // cannot find any more sku to be relocated
                            rack[x, y, z] = line;
                            Console.Write("[Relocating ({0})] the following storage line...\n", fromPopularity > toPopularity ? "Promote" : "De-promote");
                            Console.Write(line + "\n");
                        }
                    }
                }
            }

            // Traverse the Y direction
            for (int x = 0; x < xt_h; x++)
            {
                for (int y = yt_l; y < yt_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] == null)
                        {
                            // found empty target slot
                            var line = getRelocatedStorageLine(sku, xf_l, xf_h, yf_l, yf_h);
                            if (line == null) return; // cannot find any more sku to be relocated
                            rack[x, y, z] = line;
                            Console.Write("[Relocating ({0})] the following storage line...\n", fromPopularity > toPopularity ? "Promote" : "De-promote");
                            Console.Write(line + "\n");
                        }
                    }
                }
            }

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

                if (SKUToPopularity.ContainsKey(line.sku.index))
                {
                    var recorded_popularity = -1;
                    bool success = SKUToPopularity.TryGetValue(line.sku.index, out recorded_popularity);
                    Debug.Assert(success);
                    if (recorded_popularity != line.popularity)
                    {
                        // there is a popularity change, need relocation
                        SKUToPopularity[line.sku.index] = line.popularity;
                        var fromPopularity = recorded_popularity;
                        var toPopularity = line.popularity;
                        relocate(line.sku, fromPopularity, toPopularity);
                    }
                } else
                {
                    // no record yet, add to dict
                    SKUToPopularity.Add(line.sku.index, popularity);
                }
            }
            return true;
        }

        public StorageLine getRelocatedStorageLine(int target_popularity, int x_l, int x_h, int y_l, int y_h)
        {
            // First traverse the X direction
            for (int x = x_l; x < x_h; x++)
            {
                for (int y = 0; y < y_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] != null)
                        {
                            var storageLine = rack[x, y, z];
                            if (storageLine.popularity == target_popularity)
                            {
                                // we can relocate this storage line to a nearer zone
                                Console.Write("[Promoting] the following storage line...\n");
                                Console.Write(storageLine + "\n");
                                rack[x, y, z] = null;
                                return storageLine;
                            }
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
                        if (rack[x, y, z] != null)
                        {
                            var storageLine = rack[x, y, z];
                            if (storageLine.popularity == target_popularity)
                            {
                                Console.Write("[Promoting] the following storage line...\n");
                                Console.Write(storageLine + "\n");
                                rack[x, y, z] = null;
                                return storageLine;
                            }
                        } 
                    }
                }
            }

            return null;
        }


        public StorageLine getRelocatedStorageLine(SKU sku, int x_l, int x_h, int y_l, int y_h)
        {
            // First traverse the X direction
            for (int x = x_l; x < x_h; x++)
            {
                for (int y = 0; y < y_h; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        if (rack[x, y, z] != null)
                        {
                            var storageLine = rack[x, y, z];
                            if (storageLine.sku == sku)
                            {
                                // we can relocate this storage line to a nearer/further zone
                                // depending on popularity chagne
                                Console.Write("[Relocating] the following storage line...\n");
                                rack[x, y, z] = null;
                                return storageLine;
                            }
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
                        if (rack[x, y, z] != null)
                        {
                            var storageLine = rack[x, y, z];
                            if (storageLine.sku == sku)
                            {
                                Console.Write("[Relocating] the following storage line...\n");
                                Console.Write(storageLine + "\n");
                                rack[x, y, z] = null;
                                return storageLine;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void relocateFromLowerZone(int cur_x, int cur_y, int cur_z)
        {
            int zone_popularity = getPopularityByLoc(cur_x, cur_y, cur_z);
            Debug.Assert(zone_popularity > 0);

            for (int popu = zone_popularity + 1; popu <= 3; popu ++)
            {
                var (x_l, y_l, x_h, y_h) = getRangeByPopularity(popu);
                var relocatedStorageLine = getRelocatedStorageLine(zone_popularity, x_l, x_h, y_l, y_h);
                if (relocatedStorageLine != null)
                {
                    Console.Write("[Promoting] the following storage line...\n");
                    Console.Write(relocatedStorageLine + "\n");
                    rack[cur_x, cur_y, cur_z] = relocatedStorageLine;
                }
            }
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

                            // relocation from lower zone to here
                            relocateFromLowerZone(x,y,z);
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
        public StorageLine(SKU _sku, int _qty, int _weight, int _popularity)
        {
            sku = _sku;
            qty = _qty;
            weight = _weight;
            popularity = _popularity;
        }

        public SKU sku { get; set; }
        public int qty { get; set; }
        public int weight { get; set; }
        public int popularity { get; set; }

        public override String ToString()
        {
            return String.Format("SKU {0} Qty {1} Weight {2} Popularity {3}: ", sku, qty, weight, popularity);
        }

    }

}
