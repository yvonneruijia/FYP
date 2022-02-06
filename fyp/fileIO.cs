using System;
using System.IO;

namespace fyp
{
    internal class fileIO
    {
        public fileIO()
        {
            String curPath = Environment.CurrentDirectory;
            String basePath = Directory.GetParent(curPath).Parent.Parent.Parent.FullName;
            String inboundFile = Path.Join(basePath, "Inbound.csv");
            String outboundFile = Path.Join(basePath, "Outbound.csv");
            inboundReader = new StreamReader(inboundFile);
            outboundReader = new StreamReader(inboundFile);
            inboundReader.ReadLine(); // skip header
            outboundReader.ReadLine(); // skip header
            inboundBufferedLine = null;
            outboundBufferedLine = null;

        }

        ~fileIO()
        {
            inboundReader.Close();
            outboundReader.Close();
        }

        public StreamReader inboundReader;
        public StreamReader outboundReader;

        private String inboundBufferedLine;
        private String outboundBufferedLine;

        // TODO: Make sure for the same shipmentNO, time is the same
        public (TimeSpan, InboundShipment) getNextInbound()
        {            
            if (inboundReader.EndOfStream && inboundBufferedLine == null)
            {
                // no more input
                return (TimeSpan.MinValue, null);
            }

            var shipment = new InboundShipment();
            int lastShipmentNo = -1;
            TimeSpan shipmentTime = TimeSpan.MinValue;

            do
            {
                // when inboundBufferedLine != null,
                // that means we have peeked the last input
                if (inboundBufferedLine == null)
                {
                    inboundBufferedLine = inboundReader.ReadLine();
                }

                //Console.WriteLine(inboundBufferedLine);
                var fields = inboundBufferedLine.Split(",");
                var shipmentNo = int.Parse(fields[0]);
                var time = TimeSpan.Parse(fields[1]);
                var orderNo = int.Parse(fields[2]);
                var sku = int.Parse(fields[3]);
                var qty = int.Parse(fields[4]);

                if (lastShipmentNo!=-1 && lastShipmentNo != shipmentNo)
                {
                    // next line is a new shipment
                    break;
                }

                shipment.addShipmentLine(new InboundShipmentLine(new SKU(sku), qty, orderNo));

                shipmentTime = time;
                lastShipmentNo = shipmentNo;
                inboundBufferedLine = null;

            } while (!inboundReader.EndOfStream);

            return (shipmentTime, shipment);
        }

        // TODO: Make sure for the same shipmentNO, time is the same
        public (TimeSpan, OutboundOrder) getNextOutbound()
        {
            if (outboundReader.EndOfStream && outboundBufferedLine == null)
            {
                // no more input
                return (TimeSpan.MinValue, null);
            }

            var shipment = new OutboundOrder();
            int lastShipmentNo = -1;
            TimeSpan shipmentTime = TimeSpan.MinValue;

            do
            {
                // when outboundBufferedLine != null,
                // that means we have peeked the last input
                if (outboundBufferedLine == null)
                {
                    outboundBufferedLine = outboundReader.ReadLine();
                }

                //Console.WriteLine(inboundBufferedLine);
                var fields = outboundBufferedLine.Split(",");
                var shipmentNo = int.Parse(fields[0]);
                var time = TimeSpan.Parse(fields[1]);
                var orderNo = int.Parse(fields[2]);
                var sku = int.Parse(fields[3]);
                var qty = int.Parse(fields[4]);

                if (lastShipmentNo != -1 && lastShipmentNo != shipmentNo)
                {
                    // next line is a new shipment
                    break;
                }

                shipment.addOrderLine(new OutboundOrderLine(new SKU(sku), qty, orderNo));

                shipmentTime = time;
                lastShipmentNo = shipmentNo;
                outboundBufferedLine = null;

            } while (!outboundReader.EndOfStream);

            return (shipmentTime, shipment);
        }

    }
}
