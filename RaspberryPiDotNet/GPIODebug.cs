using System;

namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi GPIO debug class.
    /// </summary>
    public class GPIODebug : GPIO
    {
        private bool _currentValue = false;

        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIODebug(GPIOPins pin)
            : this(pin,GPIODirection.Out,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIODebug(GPIOPins pin, GPIODirection direction)
            : this(pin, direction, false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with the specified initial value
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        /// <param name="initialValue">Initial Value</param>
        public GPIODebug(GPIOPins pin, GPIODirection direction, bool initialValue)
            : base(pin, direction, initialValue)
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
			System.Diagnostics.Debug.WriteLine("GPIO pin " + _pin + " set to " + value);
			base.Write(value);
			_currentValue = value;
        }

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public override bool Read()
        {
			System.Diagnostics.Debug.WriteLine("GPIO pin " + _pin + " reads as " + _currentValue);
			base.Read();
            return _currentValue;
        }

        /// <summary>
        /// Dispose of the GPIO pin
        /// </summary>
        public override void Dispose()
        {
			base.Dispose();
        }
        #endregion
    }
}
