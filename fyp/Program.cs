using O2DESNet;
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
        }
    }


    internal class MySimModel : Sandbox
    {
        //void MyEvent(int count)
        //{
        //    //Console.WriteLine("{0}\tHello World {1}", ClockTime, count);

        //    var shipment = new InboundShipment();
        //    shipment.inboundShipmentLines = new List<InboundShipmentLine>()
        //    {
        //        new InboundShipmentLine(1, new SKU(1), count),
        //        new InboundShipmentLine(2, new SKU(2), 3)
        //    };
        //    shipment.listInboundShipments();

        //    // Schedule a future event
        //    Schedule(() => MyEvent(count + 1), TimeSpan.FromMinutes(1));
        //}

        public int sec = 10000000;

        public void arrival(InboundShipment shipment)
        {
            shipment.inboundTime = ClockTime;
            shipment.listInboundShipments();
            //Console.WriteLine("Arrival {0}", shipment.inboundTime);
            return;
        }


        public MySimModel()
        {
            //Schedule(() => MyEvent(1)); // schedule the initial event

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


            Schedule(() => arrival(in1), new TimeSpan(4*sec));
            Schedule(() => arrival(in2), new TimeSpan(8*sec));
            Schedule(() => arrival(in3), new TimeSpan(2*sec));
        }
    }
}
