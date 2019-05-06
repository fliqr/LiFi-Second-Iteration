using System;
using System.IO.Ports;
using System.IO;

public class DisplayArduinoOutput
{
    public DisplayArduinoOutput()
    {

    }

    public static void Main(String[] args)
    {
        SerialPort arduinoOutput = new SerialPort();
        arduinoOutput.BaudRate = 115200;
        arduinoOutput.PortName = "COM9";
        arduinoOutput.Open();
        int fullRead;
        

        while (true)
        {
            Console.WriteLine(arduinoOutput.ReadLine());
        }


    }
}

