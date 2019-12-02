using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.IO;



namespace SerialReadCSharp
{
    class Program
    {

        private static int zeroBitMultiplier = 2, oneBitMultiplier = 4, startBitMultiplier = 8;

        private static int startBitTime = -1, zeroBitTime = -1, oneBitTime = -1, offBitTime = -1;
        private static int zeroOneBitTimeCutOff = 11, startBitTimeCutOff = 25;
        private static int highValue = -1, lowValue, cutoffValue;
        private static bool isReady = false;
        private static double offCountAvg = 0;
        private static int offCountCount = 0;
        private static int highestValueOfTheSensor = 1000;

        private static String rawSerialInput = "";
        private static List<int> inputValues;
        private static StreamWriter dataFile;

        public static int timeNow()
        {
            return int.Parse(DateTime.Now.ToString("ssffffff"));
        }

        public static String ConvertFromBinary(String binaryCodeFromArduino)
        {
            String data = "";

            while (binaryCodeFromArduino.Length >= 8)
            {
                data += char.ConvertFromUtf32(Convert.ToInt32(binaryCodeFromArduino.Substring(0, 8), 2));

                binaryCodeFromArduino = binaryCodeFromArduino.Substring(8);
            }
            return data;
        }

        public static void AddToFile(String data)
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
        

        private static void calibrateMaxMin(int lightValue)
        {
            if (lightValue > highValue && lightValue < highestValueOfTheSensor) highValue = lightValue;
            if (lightValue < lowValue) lowValue = lightValue;

            cutoffValue = (highValue + lowValue) / 2;

        }

        private static bool isRising(int lastVal, int currentVal)
        {
            if (lastVal < cutoffValue && currentVal > cutoffValue)
                return true;
            return false;
        }
        private static bool isFalling(int lastVal, int currentVal)
        {
            if (lastVal > cutoffValue && currentVal < cutoffValue)
                return true;
            return false;
        }

        private static void updateOffBitTime(int offCount)
        {
            
            if(offCountCount < 5)
            {
                offCountAvg = ((offCountAvg * offCountCount++) + offCount) / offCountCount;
                return;
            }
            offBitTime = (int)(offCountAvg);
            zeroBitTime = offBitTime * zeroBitMultiplier;
            oneBitTime = zeroBitTime * oneBitMultiplier;
            startBitTime = zeroBitTime * startBitMultiplier;
            zeroOneBitTimeCutOff = (zeroBitTime + oneBitTime) / 2;
            startBitTimeCutOff = (int)((oneBitTime + startBitTime) * 0.45);

        }

        private static bool isSignal(int onCount)
        {
            if (onCount > startBitTimeCutOff)
                return false;
            return true;
        }

        private static bool isOne(int onCount)
        {
            if (onCount > zeroOneBitTimeCutOff)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /***
         * 
         * For the first 40 values, calibrate the high low and threshold values of the signal.
         * These will adapt 
         * For the next 100 values, probe for an off value. If one occurs, calibate all timing values.
         * 
         */
        private static void startup()
        {
            int lightvalue;
            int lastVal;
            int intervalCount = 0;
            int dataPoints = inputValues.Count;
            Thread compileDataThread = new Thread(new ThreadStart(compileData));


            if (dataPoints > 200)
            {
                dataPoints = 200;
            }
            for (int i = 0; i < dataPoints; i++)
            {
                lightvalue = inputValues[i];
                calibrateMaxMin(lightvalue);
            }
            lastVal = inputValues[0];
            /*
            for (int i = 1; i < 100; i++)
            {
                lightvalue = inputValues[i];
                calibrateMaxMin(lightvalue);

                if(isRising(lastVal, lightvalue))
                {
                    updateOffBitTime(intervalCount);
                    intervalCount = 1;
                } else if (isFalling(lastVal, lightvalue))
                {
                    intervalCount = 1;
                } else
                {
                    intervalCount++;
                }

                lastVal = lightvalue;
            }
            */
            Console.WriteLine(cutoffValue);
            compileDataThread.Start();

        }

        private static void compileData()
        {

            int lightvalue;
            int lastVal;
            int intervalCount = 0;
            int dataPoints = inputValues.Count;
            int bitindex = 7;
            int bitValue = 0;
            List<char> finalData = new List<char>();

            lastVal = inputValues[0];
            inputValues.RemoveAt(0);
            while (true) {
                if (inputValues.Count > 0)
                {
                    lightvalue = inputValues[0];
                    inputValues.RemoveAt(0);
                    calibrateMaxMin(lightvalue);

                    if (isRising(lastVal, lightvalue))
                    {
                        intervalCount = 1;
                    }
                    else if (isFalling(lastVal, lightvalue))
                    {
                        if (isSignal(intervalCount))
                        {
                            if (isOne(intervalCount))
                            {
                                bitValue += (int)(Math.Pow(2, bitindex));

                                Console.Write("1");
                            }
                            else
                            {
                                Console.Write("0");
                            }
                            bitindex--;
                            if (bitindex <=-1)
                            {
                                finalData.Add(Convert.ToChar(bitValue));
                                bitValue = 0;
                                bitindex = 7;
                            }
                        }
                        else
                        {
                            String finalDataString = "";
                            for (int i = 0; i < finalData.Count; i++) {
                                finalDataString += finalData[i];
                            }
                            AddToFile(finalDataString);
                            Console.WriteLine(finalDataString);
                            finalData.Clear();
                        }
                        intervalCount = 1;
                    }
                    else
                    {
                        intervalCount++;
                    }

                    lastVal = lightvalue;
                }
            }
        }


        private static void parseString()
        {
            String[] splitted;
            if (!rawSerialInput.Equals(""))
            {
                //convert the string from serial input to a list of ints
                splitted = rawSerialInput.Split();
                for (int i = 0; i < splitted.Length; i++)
                {
                    if (splitted[i] != "")
                    {
                        try
                        {
                            inputValues.Add(int.Parse(splitted[i]));
                        }
                        catch
                        {
                            Console.WriteLine(splitted[i] + " is not parsable");
                        }
                    }
                }
                //if the light values are not calibrated yet, calibte them
                if (highValue == -1)
                {
                    startup();
                } 

            }
        }
        

        private static void parseRawData(SerialPort arduinoOut)
        {
            int startTime = timeNow();
            while (true)
            {
                try
                {
                    rawSerialInput = arduinoOut.ReadExisting();
                    parseString();
                }
                catch
                {
                    Console.WriteLine("Serial could not be read.");
                }
            }
        }


        static void Main(string[] args)
        {

            //var path = "C:\\Users\\Mike\\Documents\\Arduino\\IEEE Hackathon\\Data.txt";
            //var path = "D:\\Users\\mike\\Documents\\[school]\\Indipendant projects\\Data.txt";
            String path ="Data.txt";
            SerialPort arduinoOut = new SerialPort();
            dataFile = new StreamWriter(path);
            inputValues = new List<int>();
            arduinoOut.BaudRate = 115200;
            arduinoOut.PortName = "COM10";
            arduinoOut.Open();

            parseRawData(arduinoOut);
            
            

                //the value in Ticks represents the time IN 100 nanoseconds since 1.1.1970 but my computer clock is only about 10ms fast. 
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

            
        }
    }
}
