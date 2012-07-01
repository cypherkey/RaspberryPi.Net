using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Author: Aaron Anderson <aanderson@netopia.ca>
namespace RaspberryPiDotNet
{
    /// <summary>
    /// Abstract class for the GPIO connector on the Pi (P1) (as found next to the yellow RCA video socket on the Rpi circuit board)
    /// </summary>
    public abstract class GPIO : IDisposable
    {
        /// <remarks>
        /// Refer to http://elinux.org/Rpi_Low-level_peripherals for diagram.
        /// P1-01 = bottom left, P1-02 = top left
        /// pi connector P1 pin    = GPIOnum
        ///                  P1-03 = GPIO0
        ///                  P1-05 = GPIO1
        ///                  P1-07 = GPIO4
        ///                  P1-08 = GPIO14 - alt function (UART0_TXD) on boot-up
        ///                  P1-10 = GPIO15 - alt function (UART0_TXD) on boot-up
        ///                  P1-11 = GPIO17
        ///                  P1-12 = GPIO18
        ///                  P1-13 = GPIO21
        ///                  P1-15 = GPIO22
        ///                  P1-16 = GPIO23
        ///                  P1-18 = GPIO24
        ///                  P1-19 = GPIO10
        ///                  P1-21 = GPIO9
        ///                  P1-22 = GPIO25
        ///                  P1-23 = GPIO11
        ///                  P1-24 = GPIO8
        ///                  P1-26 = GPIO7
        /// So to turn on Pin7 on the GPIO connector, pass in enumGPIOPIN.gpio4 as the pin parameter
        /// </remarks>
        public enum GPIOPins
        {
            GPIO_NONE = -1,
            GPIO00 = 0,
            GPIO01 = 1,
            GPIO04 = 4,
            GPIO07 = 7,
            GPIO08 = 8,
            GPIO09 = 9,
            GPIO10 = 10,
            GPIO11 = 11,
            GPIO14 = 14,
            GPIO15 = 15,
            GPIO17 = 17,
            GPIO18 = 18,
            GPIO21 = 21,
            GPIO22 = 22,
            GPIO23 = 23,
            GPIO24 = 24,
            GPIO25 = 25,
            Pin03 = 0,
            Pin05 = 1,
            Pin07 = 4,
            Pin08 = 14,
            Pin10 = 15,
            Pin11 = 17,
            Pin12 = 18,
            Pin13 = 21,
            Pin15 = 22,
            Pin16 = 23,
            Pin18 = 24,
            Pin19 = 10,
            Pin21 = 9,
            Pin22 = 25,
            Pin23 = 11,
            Pin24 = 8,
            Pin26 = 7
        };

        /// <summary>
        /// Specifies the direction of the GPIO port
        /// </summary>
        public enum DirectionEnum { IN, OUT };

        /// <summary>
        /// Dictionary that stores whether a pin is setup or not
        /// </summary>
        protected static Dictionary<int, DirectionEnum> _exportedPins = new Dictionary<int, DirectionEnum>();

        /// <summary>
        /// The currently assigned GPIO pin. Used for class methods and not static methods.
        /// </summary>
        protected GPIOPins _pin;

        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIO(GPIOPins pin)
            : this(pin,DirectionEnum.OUT,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIO(GPIOPins pin, DirectionEnum direction)
            : this(pin, direction, false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with the specified initial value
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        /// <param name="initialValue">Initial Value</param>
        public GPIO(GPIOPins pin, DirectionEnum direction, bool initialValue)
        {
            this._pin = pin;
        }

        protected static string GetGPIONumber(GPIOPins pin)
        {
            return ((int)pin).ToString(); //e.g. returns 17 for enum value of gpio17
        }

        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        public abstract void Write(bool value);

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public abstract bool Read();

        /// <summary>
        /// Dispose of the GPIO pin
        /// </summary>
        public abstract void Dispose();
    }
}
