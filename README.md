RaspberryPi.Net
===============

Introduction
------------
The purpose of this library is to provide a Mono.NET interface to the GPIO pins
on the Raspberry Pi. All of this code was written using Visual Studio 2010
Express but the goal is to be fully compatible with Mono. This library is
written using .NET 4.0 therefore the latest version of Mono (2.10) is
recommended. At the time of this update, the Raspbian wheezy 2012-07-15 image
installs Mono 2.10.8.1.

The GPIO pins are best described
[here](http://elinux.org/Rpi_Low-level_peripherals#General_Purpose_Input.2FOutput_.28GPIO.29).
They can be accessed in 2 ways, either using the file-based I/O (GPIOFile.cs)
or direct memory (GPIOMem.cs) using Mike McCauley's BCM2835 library which is
available [here](http://www.open.com.au/mikem/bcm2835/index.html). There is
also a GPIODebug.cs class that can be used to test your application without a
Raspberry Pi.

Here is a sample bit of code to blink an LED attached to pin 12
```C#
using System;
using RaspberryPiDotNet;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            GPIOMem led = new GPIOMem(GPIOPins.V2_GPIO_12)
            while(true)
            {
                led.Write(PinState.High);
                System.Threading.Thread.Sleep(500);
                led.Write(PinState.Low);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
```

Installing Mono
---------------
To install Mono on your Raspberry Pi, run the following:
```bash
$ sudo aptitude update 
$ sudo aptitude install mono-runtime
```

My preference is for aptitude, however, apt-get can also be used.

Using GPIOMem
-------------
The GPIOMem class uses the .NET Interop layer to expose C functions from Mike
McCauley''s BCM2835 library. This requires the use of a separate shared object
(.so) but this library is considerably faster than the GPIOFile method.

The Makefile for his library compiles a shared object where a statically linked
library is required. To compile a statically linked binary, do the following:

```bash
# tar -zxf bcm2835-1.3.tar.gz
# cd bcm2835-1.3/src
# make libbcm2835.a
# cc -shared bcm2835.o -o libbcm2835.so
```

You can also try using our own compiled file [here](https://www.dropbox.com/s/716brdf0owbx4tf/libbcm2835.so). If it doesn't work, you must compile yourself. It must be in the same folder as the application.

Liquid Crystal Display
----------------------
This class is a port of the MicroLiquidCrystal NetDuino library from
[here](http://microliquidcrystal.codeplex.com). It provides an interface to
address HD44780 compatible displays. 

Example code:
```C#
RaspPiGPIOMemLcdTransferProvider lcdProvider = new RaspPiGPIOMemLcdTransferProvider(
    GPIOPins.Pin_P1_21,
    GPIOPins.Pin_P1_18,
    GPIOPins.Pin_P1_11,
    GPIOPins.Pin_P1_13,
    GPIOPins.Pin_P1_15,
    GPIOPins.Pin_P1_19);


Lcd lcd = new Lcd(lcdProvider);
lcd.Begin(16, 2);
lcd.Clear();
lcd.SetCursorPosition(0, 0);
lcd.Write("Hello World!");
```

Using MCP3008
-------------
This class is a port of a Python Script by Mikey Sklar
[here](https://raw.github.com/gist/3249416/7689f68f3ddbb74aceecda23e395c729668bd520/adafruit-cosm-temp.py).
It provides analog to digital conversion to the Raspberry Pi.

The following example shows how to connect an analog temperature sensor to the Pi.

Example code:
```C#
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RaspberryPiDotNet;

namespace RPi_Temperature
{
    class Program
    {
        static void Main(string[] args)
        {
            //# set up the SPI interface pins
            //# SPI port on the ADC to the Cobbler
            GPIOMem SPICLK = new GPIOMem(GPIOPins.Pin_P1_18, GPIODirection.Out);
            GPIOMem SPIMISO = new GPIOMem(GPIOPins.Pin_P1_23, GPIODirection.In);
            GPIOMem SPIMOSI = new GPIOMem(GPIOPins.Pin_P1_24, GPIODirection.Out);
            GPIOMem SPICS = new GPIOMem(GPIOPins.Pin_P1_22, GPIODirection.Out);

            // temperature sensor connected to channel 0 of mcp3008
            int adcnum = 0;
            double read_adc0 = 0.0;

            while (true)
            {
                MCP3008 MCP3008 = new MCP3008(adcnum, SPICLK, SPIMOSI, SPIMISO, SPICS);
                // read the analog pin (temperature sensor LM35)
                read_adc0 = MCP3008.AnalogToDigital;
                double millivolts = Convert.ToDouble(read_adc0) * (3300.0 / 1024);

                double volts = (Convert.ToDouble(read_adc0) / 1024.0f) * 3.3f;
                double temp_C = ((millivolts - 100.0) / 10.0) - 40.0;
                double temp_F = (temp_C * 9.0 / 5.0) + 32;

#if DEBUG
                System.Console.WriteLine("MCP3008_Channel: " + adcnum);
                System.Console.WriteLine("read_adc0: " + read_adc0);
                System.Console.WriteLine("millivolts: " + (float)millivolts);
                System.Console.WriteLine("tempC: " + (float)temp_C);
                System.Console.WriteLine("tempF: " + (float)temp_F);
                System.Console.WriteLine("volts: " + (float)volts);
		//The following line makes the trick on Raspberry Pi for displaying DateTime.Now
		//equivalent.
		Console.WriteLine("Date time stamp: {0}/{1}/{2} {3}:{4}:{5}",now.Month,now.Day,now.Year,
		                  now.Hour,now.Minute,now.Second);
		System.Console.WriteLine("\n");
#endif
                Thread.Sleep(3000);
            }
        }
    }
}
```
