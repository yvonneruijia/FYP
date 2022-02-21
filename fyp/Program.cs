using O2DESNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace fyp
{
    class Program
    {
        static void Main(string[] args)
        {
            // construct the model
            var xa = new int[] { 1, 3, 2, 5, 2 };
            var ya = new int[] { 2, 1, 3, 3, 4 };
            var xb = new int[] { 5, 4, 3, 6, 4 };
            var yb = new int[] { 5, 4, 5, 5, 7 };
            var xc = new int[] { 10,10,10,10,10 };
            var yc = new int[] { 10,10,10,10,10 };
            for (int i = 0; i < xa.Length; i++)
            {
                var sim = new MySimModel(xa[i],ya[i],xb[i],yb[i],xc[i],yc[i],true);
                //var sim = new MySimModel(i,i,i,i,i,i,false);
                // run 2000 events
                Console.WriteLine("[SIM STARTS] (({0},{1}),({2},{3}),({4},{5}))\n",xa[i],ya[i],xb[i],yb[i],xc[i],yc[i]);
                sim.Run(5000); //update this when there's more arrival and outbound events
                Console.WriteLine("[STATS] Total Distance Travelled: {0}\n", sim.getTotalDistanceTravelled());
                Console.WriteLine("[SIM ENDS] Run {0} events", sim.CountEvent);

            }
            Console.WriteLine("[PROGRAM ENDS]");


        }
    }


    internal class MySimModel : Sandbox
    {

        public int sec = 10000000;

        public Storage storage;
        public bool isClassBased;
        public int CountEvent = 0;
        public MySimModel(int xa,int ya,int xb, int yb, int xc, int yc,bool _isClassBased)
        {
            isClassBased = _isClassBased;
            storage = new Storage(10,10,10,xa,ya,xb,yb,xc,yc,isClassBased); // Create new storage

            var io = new fileIO();

            // register inbound
            while (true)
            {
                var (time, inbound) = io.getNextInbound();
                if (inbound == null)
                {
                    break;
                }
                //Console.WriteLine(time)
                Schedule(() => arrive(inbound), time);
            };

            // register outbound
            while (true)
            {
                var (time, outbound) = io.getNextOutbound();
                if (outbound == null)
                {
                    break;
                }
                //Console.WriteLine(time)
                Schedule(() => order(outbound), time);
            };

        }

        public void order(OutboundOrder order)
        {
            //Console.WriteLine("Picking an order...\n");
            Debug.Assert(order != null);
            order.outboundTime = ClockTime;
            order.listOutboundOrders();
            storage.pickStorage(order);
            CountEvent++;
        }

        public void arrive(InboundShipment shipment)
        {
            //Console.WriteLine("Order arriving...\n");
            Debug.Assert(shipment != null);
            shipment.inboundTime = ClockTime;
            shipment.listInboundShipments();
            CountEvent++;
            bool success = false;
            if (isClassBased)
            {
                success = storage.assignStorageClassBased(shipment);
            }
            else
            {
                success = storage.assignStorageRandom(shipment);
            }

            if (!success)
            {
                Console.WriteLine("Storage is full / need relocation, exiting...");
                Environment.Exit(0);
            }

        }

        public int getTotalDistanceTravelled()
        {
            return storage.totalDistance;
        }

        public void generateTestInputs()
        {
            // Create three inbound shipments
            var in1 = new InboundShipment();
            var in2 = new InboundShipment();
            var in3 = new InboundShipment();

            in1.addShipmentLine(new InboundShipmentLine(new SKU(1), 5, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(2), 10, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(3), 2, -1));

            in2.addShipmentLine(new InboundShipmentLine(new SKU(2), 7, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(3), 10, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(4), 1, -1));

            in3.addShipmentLine(new InboundShipmentLine(new SKU(1), 5, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(4), 10, -1))
               .addShipmentLine(new InboundShipmentLine(new SKU(5), 6, -1));


            // Create three inbound shipments
            var out1 = new OutboundOrder();
            var out2 = new OutboundOrder();
            var out3 = new OutboundOrder();

            out1.addOrderLine(new OutboundOrderLine(new SKU(1), 5, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(2), 10, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(3), 2, -1));

            out2.addOrderLine(new OutboundOrderLine(new SKU(2), 7, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(3), 10, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(4), 1, -1));

            out3.addOrderLine(new OutboundOrderLine(new SKU(1), 5, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(4), 10, -1))
               .addOrderLine(new OutboundOrderLine(new SKU(5), 6, -1));


            // Schedule 3 arrival events and 3 order events
            Schedule(() => arrive(in1), new TimeSpan(4 * sec));
            Schedule(() => arrive(in2), new TimeSpan(8 * sec));
            Schedule(() => arrive(in3), new TimeSpan(2 * sec));

            Schedule(() => order(out1), new TimeSpan(5 * sec));
            Schedule(() => order(out2), new TimeSpan(9 * sec));
            Schedule(() => order(out3), new TimeSpan(3 * sec));

        }

    }
}
