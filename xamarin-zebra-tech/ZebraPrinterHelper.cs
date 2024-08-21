using System;
using System.Text;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;

namespace xamarin_zebra_tech
{
    public class ZebraPrinterHelper
    {
        private ZebraPrinter printer;
        private Connection connection;

        public ZebraPrinterHelper(string macAddress)
        {
            // Establish a Bluetooth connection
            connection = new BluetoothConnection(macAddress);
        }

        public void Connect()
        {
            try
            {
                connection.Open();
                printer = ZebraPrinterFactory.GetInstance(connection);
                //PrinterSettings settings = printer.GetPrinterSettings();
                // You can configure printer settings if needed
            }
            catch (ConnectionException e)
            {
                // Handle exceptions
                Console.WriteLine($"Connection error: {e.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                connection.Close();
            }
            catch (ConnectionException e)
            {
                // Handle exceptions
                Console.WriteLine($"Disconnection error: {e.Message}");
            }
        }

        public void PrintText(string text)
        {
            try
            {
                if (printer != null)
                {
                    // Create a ZPL command for printing text
                    string zpl = "^XA^FO50,50^ADN,36,20^FD" + text + "^FS^XZ";
                    connection.Write(Encoding.UTF8.GetBytes(zpl));
                }
            }
            catch (ConnectionException e)
            {
                // Handle exceptions
                Console.WriteLine($"Print error: {e.Message}");
            }
        }
    }
}