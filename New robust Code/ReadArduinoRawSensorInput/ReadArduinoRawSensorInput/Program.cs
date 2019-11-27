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
        
        private static int startBitTime, zeroBitTime, oneBitTime, offBitTime;
        private static int highValue, lowValue, cutoffValue;
        private static bool isReady = false;

        private static LinkedList<int> offTime;

        public static int timeNow()
        {
            return int.Parse(DateTime.Now.ToString("ssffffff"));
        }

        public static String ConvertFromBinary(String binaryCodeFromArduino)
        {
            String data = "";
            binaryCodeFromArduino = binaryCodeFromArduino.Substring(4);

            while (binaryCodeFromArduino.Length >= 8)
            {
                data += char.ConvertFromUtf32(Convert.ToInt32(binaryCodeFromArduino.Substring(0, 8), 2));

                binaryCodeFromArduino = binaryCodeFromArduino.Substring(8);
            }

            Console.WriteLine(data);
            return data;
        }

        public static void AddToFile(TextWriter dataFile, String data)
        {
            try
            {
                dataFile.Write(data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
            }

        }
        

        private static void calibrateMaxMin(SerialPort arduinoOut)
        {

            int timeStart = timeNow();

            while ((timeStart - timeNow()) < 50000)
            {
                try
                {
                    int lightValue = int.Parse(arduinoOut.ReadLine());
                    if (lightValue > highValue) highValue = lightValue;
                    if (lightValue < lowValue) lowValue = lightValue;

                    cutoffValue = (highValue + lowValue) / 2;
                }
                catch
                {
                    Console.WriteLine("No value");
                }
            }

            Console.WriteLine("High value: " + highValue);
            Console.WriteLine("Low value: " + lowValue);

        }
        
        

        private static void parseRawData(SerialPort arduinoOut)
        {
            int startTime = timeNow();
            while (true)
            {
                int lightValue = int.Parse(arduinoOut.ReadLine());
                
            }
        }


        static void Main(string[] args)
        {




            //var path = "C:\\Users\\Mike\\Documents\\Arduino\\IEEE Hackathon\\Data.txt";
            var path = "D:\\Users\\mike\\Documents\\[school]\\Indipendant projects\\Data.txt";
            SerialPort arduinoOut = new SerialPort();
            TextWriter dataFile = new StreamWriter(path, true);
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM10";
            arduinoOut.Open();

            offTime = new LinkedList<int>();
            

            long teeeeemp = DateTime.Now.Ticks;
            long teeemp = DateTime.Now.Ticks;
            Console.WriteLine(teeemp - teeeeemp);
            

            /**
             * 
             * 
             * Conclusions:
             * 1) The ADC on ANY arduino can only sample every 0.0003 seconds or 300us. This is due to the readADC function taking a long time to run. Doesnt matter the clock speed of the device
             * 2) SerialPort.ReadExisting reads the current data in the stream.
             * 3) THE STRING TO INT PARSE TAKES LIKE 0.03 SECONDS THATS WAY TOO FUCKING LONG
             * 
             * 
             */
            /*
                   String[] temp = new string[100];
                   String timeNow = DateTime.Now.ToString("ssffffff");
                   for(int i = 0; i < temp.Length; i++)
                   {
                       temp[i] = arduinoOut.ReadLine();
                   }
                   String timeDone = DateTime.Now.ToString("ffffff");
                   int totalTime = int.Parse(timeDone) - int.Parse(timeNow); 
           */



            calibrateMaxMin(arduinoOut);
            parseRawData(arduinoOut);
            
        }

        
    }
}
