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

        public static Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        public static void AddToFile(TextWriter dataFile, String data)
        {
            Console.WriteLine("   " + data.ToString());
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
            SerialPort arduinoOut = new SerialPort();
            TextWriter dataFile = new StreamWriter(path, true);
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM7";
            arduinoOut.Open();
            String fullByte = "";
            String bitFromArduino = "";
            var data = "";
            dataFile = new StreamWriter(path, true);                            //fills the dataFile object with the path


            //assuming this code reads serial port more often than arduino sends it
            while (true)
            {

                bitFromArduino = arduinoOut.ReadExisting();                            //reads from serial
                if (bitFromArduino == "D")                                             //if the input is the string D, then it is the start of a new thing
                {
                    bitFromArduino = "";                                                //one single bit (1/0) from the serial port 
                    fullByte = "";                                                      //the compilataion of 8 bits from the arduino
                    while (bitFromArduino != "D")                                       //Keep running the loop until the end of the message "D" is received
                    {
                        //code to continuously write to the file.
                        bitFromArduino = arduinoOut.ReadExisting();                     //puts the actual serial value from the arduino in bitFromArduino
                        if (fullByte.Length < 8)                                        //if there are less than 8 elements in the list fullByte than the Serial data from the arduino will be added
                        {
                            fullByte += bitFromArduino;
                            Console.Write(bitFromArduino);
                        } else {                                      //once the byte has 8 bits, translate it into an ASCII character and save that to a file
                            data = Encoding.ASCII.GetString(GetBytesFromBinaryString(fullByte.Substring(0,8)));
                            if(fullByte.Length > 8)
                            {
                                while (fullByte.Length > 8)
                                {
                                    fullByte = fullByte.Substring(8, fullByte.Length);
                                    data += Encoding.ASCII.GetString(GetBytesFromBinaryString(fullByte.Substring(0, 8)));
                                }
                            } else
                            {
                                fullByte = "";
                            }
                           
                        }
                    }
                }
                AddToFile(dataFile, data);
                dataFile.Close();
            }




        }
    }
}
