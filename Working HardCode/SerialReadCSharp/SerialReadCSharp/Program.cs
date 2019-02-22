﻿using System;
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
            binaryCodeFromArduino = binaryCodeFromArduino.Substring(3);

            while(binaryCodeFromArduino.Length >= 8)
            {
                data += char.ConvertFromUtf32(Convert.ToInt32(binaryCodeFromArduino.Substring(0,8), 2));

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
            String dataFromArduino = "";

            while (true)
            {
                dataFromArduino = arduinoOut.ReadExisting();
                fullByte += dataFromArduino;
                if (dataFromArduino.IndexOf("D") != -1)
                {
                    AddToFile(dataFile, ConvertFromBinary(fullByte.Substring(0, fullByte.IndexOf("D"))));
                    fullByte = fullByte.Substring(fullByte.IndexOf("D") + 1);
                }
            }
        }
    }
}