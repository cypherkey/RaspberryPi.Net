using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi GPIO debug class.
    /// </summary>
    public class GPIODebug : GPIO, IDisposable
    {
        private bool _currentValue = false;

        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIODebug(GPIOPins pin)
            : base(pin,DirectionEnum.OUT,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIODebug(GPIOPins pin, DirectionEnum direction)
            : base(pin, direction, false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with the specified initial value
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        /// <param name="initialValue">Initial Value</param>
        public GPIODebug(GPIOPins pin, DirectionEnum direction, bool initialValue)
            : base(pin, direction, initialValue)
        {
            Write(pin, initialValue);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Export the GPIO setting the direction. This creates the /sys/class/gpio/gpioXX directory.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction"></param>
        private static void ExportPin(GPIOPins pin, DirectionEnum direction)
        {
        }

        /// <summary>
        /// Unexport the GPIO. This removes the /sys/class/gpio/gpioXX directory.
        /// </summary>
        /// <param name="pin">The pin to unexport</param>
        private static void UnexportPin(GPIOPins pin)
        {
        }

        /// <summary>
        /// Static method to write a value to the specified pin. Does nothing when using the GPIODebug class.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="value">The value to write to the pin</param>
        public static void Write(GPIOPins pin, bool value)
        {
        }

        /// <summary>
        /// Static method to read a value to the specified pin. Always returns false when using the GPIODebug class.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <returns>The value read from the pin</returns>
        public static bool Read(GPIOPins pin)
        {
            return false;
        }

        public static void CleanUp()
        {
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        public override void Write(bool value)
        {
            _currentValue = value;
        }

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public override bool Read()
        {
            return _currentValue;
        }

        /// <summary>
        /// Dispose of the GPIO pin
        /// </summary>
        public override void Dispose()
        {
            UnexportPin(_pin);
        }
        #endregion
    }
}
