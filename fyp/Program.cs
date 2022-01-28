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

        void arrival(InboundShipment shipment)
        {
            Console.WriteLine("Arrival {0}", ClockTime);
            return;
        }


        public MySimModel()
        {
            //Schedule(() => MyEvent(1)); // schedule the initial event
            var sec = 10000000;
            Schedule(() => arrival(new InboundShipment()), new TimeSpan(4*sec));
            Schedule(() => arrival(new InboundShipment()), new TimeSpan(8*sec));
            Schedule(() => arrival(new InboundShipment()), new TimeSpan(2*sec));
        }
    }
}
