﻿using O2DESNet;
using System;
using System.Collections.Generic;


namespace fyp
{
    class Program
    {
        static void Main(string[] args)
        {
            // construct the model
            var sim = new MySimModel();

            // run 15 events
            sim.Run(15);
            Console.WriteLine("Simulation Successful...");

        }
    }


    internal class MySimModel : Sandbox
    {

        public int sec = 10000000;

        public Storage storage;

        public void arrive(InboundShipment shipment)
        {
            shipment.inboundTime = ClockTime;
            shipment.listInboundShipments();

            bool success = storage.assignStorage(shipment);
            if (!success)
            {
                Console.WriteLine("Storage is full, exiting...");
                Environment.Exit(0);
            }

        }

        public void order(OutboundOrder order)
        {
            Console.WriteLine("Picking an order...");
            order.listOutboundOrders();
            storage.pickStorage(order);
        }

        public MySimModel()
        {
            storage = new Storage(3,3,3); // Create new storage

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
            Schedule(() => arrive(in1), new TimeSpan(4*sec));
            Schedule(() => arrive(in2), new TimeSpan(8*sec));
            Schedule(() => arrive(in3), new TimeSpan(2*sec));

            Schedule(() => order(out1), new TimeSpan(5 * sec));
            Schedule(() => order(out2), new TimeSpan(9 * sec));
            Schedule(() => order(out3), new TimeSpan(3 * sec));
        }
    }
}
