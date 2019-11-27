using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;



namespace SerialReadCSharp
{
    class Program
    {

        public static String ConvertFromBinary(String binaryCodeFromArduino)
        {
            String data = "";
            //binaryCodeFromArduino = binaryCodeFromArduino.Substring(4);

            while(binaryCodeFromArduino.Length >= 8)
            {
                data += char.ConvertFromUtf32(Convert.ToInt32(binaryCodeFromArduino.Substring(0,8), 2));

                binaryCodeFromArduino = binaryCodeFromArduino.Substring(8);
            }
            
            Console.WriteLine(data);
            return data;
        }

        public static void AddToFile(String path, String data)
        {
            TextWriter dataFile = new StreamWriter(path, true);
            try
            {
                dataFile.Write(data);
                Console.WriteLine("Writing " + data + " to file.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                dataFile.Close();
            }

        }


        static void Main(string[] args)
        {
            //var path = "C:\\Users\\Mike\\Documents\\Arduino\\IEEE Hackathon\\Data.txt";
            //var path = "D:\\Users\\mike\\Documents\\[school]\\Indipendant projects\\Fliqr\\Data.txt";
            var fileName = "Data.txt";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            SerialPort arduinoOut = new SerialPort();
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM9";
            arduinoOut.Open();
            String fullByte = "";
            String dataFromArduino = "";

            while (true)
            {
                dataFromArduino = arduinoOut.ReadExisting();
                fullByte += dataFromArduino;
                Console.WriteLine(dataFromArduino);
                if (dataFromArduino.IndexOf("D") != -1)
                {
                    AddToFile(path, ConvertFromBinary(fullByte.Substring(0, fullByte.IndexOf("D"))));
                    fullByte = fullByte.Substring(fullByte.IndexOf("D") + 1);
                }
            }
        }
    }
}
