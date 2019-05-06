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


        static void Main(string[] args)
        {
            //var path = "C:\\Users\\Mike\\Documents\\Arduino\\IEEE Hackathon\\Data.txt";
            var path = "D:\\Users\\mike\\Documents\\[school]\\Indipendant projects\\Data.txt";
            SerialPort arduinoOut = new SerialPort();
            TextWriter dataFile = new StreamWriter(path, true);
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM9";
            arduinoOut.Open();
            String fullByte = "";
            int dataFromArduino = "";

            int solarInput;
            int intervalCounter;
            String inputString = "";
            int inputStringIndex = 0;
            int cutoffValue = 750;            //This value changes depending on the light of the room
            int startBit = 0;
            bool codeIsDone = false;


            while (true)
            {
                dataFromArduino = arduinoOut.ReadExisting();
                //sequence is starting. this is the time of the startBit
                while (dataFromArduino < cutoffValue)
                {
                    intervalCounter++;
                }
                startBit = intervalCounter;
                //Serial.print(startBit);
                //Serial.println("************");


                //general code for the rest of the signal
                while (intervalCounter <= startBit && startBit > 30)
                {
                    intervalCounter = 0;
                    //high signal

                    while (analogRead(A0) > cutoffValue)
                    {
                        intervalCounter++;
                    }
                    //Serial.print("On:");
                    //Serial.println(intervalCounter);
                    if (intervalCounter > startBit)
                    {
                        codeIsDone = true;
                        break;
                    }
                    if (intervalCounter > 15)
                    {
                        Serial.print("1");
                        inputString.concat("1");
                    }
                    else
                    {

                        Serial.print("0");
                        inputString.concat("0");
                    }

                    intervalCounter = 0;

                    while (analogRead(A0) <= cutoffValue)
                    {
                        intervalCounter++;
                    }
                    //Serial.print("Off:");
                    //Serial.println(intervalCounter);
                }
                if (codeIsDone || intervalCounter > startBit)
                {
                    intervalCounter = 0;
                    //Serial.println("Signal over");
                    //Serial.println(inputString);
                    Serial.print("D");
                    inputString = "";
                    //signal has ended
                }

            }
        }
    }
}
