using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

//USE THIS 
//USE THIS
//USE THIS 
//USE THIS
//USE THIS 
//USE THIS
//USE THIS 
//USE THIS
//USE THIS 
//USE THIS
//USE THIS 
//USE THIS



namespace SerialReadCSharp
{
    class Program
    {

        public static String ConvertFromBinary(String binaryCodeFromArduino)
        {
            String data = "";
            Console.WriteLine(binaryCodeFromArduino);
            binaryCodeFromArduino = binaryCodeFromArduino.Substring(5);

            while(binaryCodeFromArduino.Length > 8)
            {
                data += char.ConvertFromUtf32(Convert.ToInt32(binaryCodeFromArduino.Substring(0,8), 2));

                binaryCodeFromArduino = binaryCodeFromArduino.Substring(8);
            }
            
            Console.WriteLine(data);
            return data;
        }

        public static void AddToFile(TextWriter dataFile, String data)
        {
            //Console.WriteLine("   " + data.ToString());
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
                //Console.WriteLine("Executing finally block.");
            }

        }


        static void Main(string[] args)
        {

            var path = "C:\\Users\\Mike\\Documents\\Arduino\\IEEE Hackathon\\Data.txt";
            //var path = "D:\\Users\\mike\\Documents\\[school]\\Indipendant projects\\Data.txt";
            SerialPort arduinoOut = new SerialPort();
            TextWriter dataFile = new StreamWriter(path, true);
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM7";
            arduinoOut.Open();
            String fullByte = "";
            String dataFromArduino = "";
            //dataFile = new StreamWriter(path, true);                            //fills the dataFile object with the path



       

            //assuming this code reads serial port more often than arduino sends it
            while (true)
            {
                dataFromArduino = arduinoOut.ReadExisting();
                fullByte += dataFromArduino;
                if (dataFromArduino.IndexOf("D") != -1) {
                    AddToFile(dataFile, ConvertFromBinary(fullByte.Substring(0, fullByte.IndexOf("D"))));
                    fullByte = fullByte.Substring(fullByte.IndexOf("D") + 1);
                }



                /*
                    dataFromArduino = arduinoOut.ReadExisting();                            //reads from serial
                if (dataFromArduino.IndexOf("D") != -1)                              //if the input is the string D, then it is the start of a new thing
                {
                    fullByte = dataFromArduino.Substring(dataFromArduino.IndexOf("D"),dataFromArduino.Length); //the compilataion of 8 bits from the arduino
                                                                                        //That code is needed because dataFromArduino will sometimes be multaple bits so the fullByte must be a compilation of them
                    dataFromArduino = "";                                                //one single bit (1/0) from the serial port 

                    while (dataFromArduino.IndexOf("D") != -1)                                       //Keep running the loop until the end of the message "D" is received
                    {
                        //code to continuously write to the file.
                        dataFromArduino = arduinoOut.ReadExisting();                     //puts the actual serial value from the arduino in dataFromArduino
                        if (fullByte.Length < 8)                                        //if there are less than 8 elements in the list fullByte than the Serial data from the arduino will be added
                        {
                            fullByte += dataFromArduino;
                            Console.Write(dataFromArduino);
                        }
                        else
                        {                                                        //once the byte has 8 bits, translate it into an ASCII character and save that to a file
                            
                            while (fullByte.Length >= 8)
                            {
                                data += Encoding.ASCII.GetString(GetBytesFromBinaryString(fullByte.Substring(0, 8)));
                                fullByte = fullByte.Substring(8, fullByte.Length - 8);
                            }

                        }
                    }
                }
                */
            }
        }
    }
}
