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
            GPIOMem led = new GPIOMem(GPIO.GPIOPins.Pin12);
            while(true)
            {
                led.Write(true);
                System.Threading.Thread.Sleep(500);
                led.Write(false);
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

Liquid Crystal Display
----------------------
This class is a port of the MicroLiquidCrystal NetDuino library from
[here](http://microliquidcrystal.codeplex.com). It provides an interface to
address HD44780 compatible displays. 

Example code:
```C#
RaspPiGPIOMemLcdTransferProvider lcdProvider = new RaspPiGPIOMemLcdTransferProvider(
    GPIOFile.GPIOPins.Pin21,
    GPIOFile.GPIOPins.Pin23,
    GPIOFile.GPIOPins.Pin11,
    GPIOFile.GPIOPins.Pin13,
    GPIOFile.GPIOPins.Pin15,
    GPIOFile.GPIOPins.Pin19);

Lcd lcd = new Lcd(lcdProvider);
lcd.Begin(16, 2);
lcd.Clear();
lcd.SetCursorPosition(0, 0);
lcd.Write("Hello World!");
```
