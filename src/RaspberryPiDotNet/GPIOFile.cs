using System.IO;

// Author: Aaron Anderson <aanderson@netopia.ca>
// Based on work done by x4m and britguy (http://www.raspberrypi.org/phpBB3/viewtopic.php?f=34&t=6720)

namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi GPIO using the file-based access method.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GPIOFile : GPIO
    {
        /// <summary>
        /// The path on the Raspberry Pi for the GPIO interface
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const string GPIO_PATH = "/sys/class/gpio/";

        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIOFile(GPIOPins pin)
            : this(pin,GPIODirection.Out,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIOFile(GPIOPins pin, GPIODirection direction)
            : this(pin, direction, false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with the specified initial value
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        /// <param name="initialValue">Initial Value</param>
        public GPIOFile(GPIOPins pin, GPIODirection direction, bool initialValue)
            : base(pin, direction, initialValue)
        {
        }

        #endregion

		#region Properties

		/// <summary>
		/// Gets or sets the communication direction for this pin
		/// </summary>
		public override GPIODirection PinDirection
		{
			get
			{
				return base.PinDirection;
			}
			set
			{
				if (PinDirection != (base.PinDirection = value)) // Left to right eval ensures base class gets to check for disposed object access
				{
					if (!Directory.Exists(GPIO_PATH + "gpio" + (uint)_pin))
						File.WriteAllText(GPIO_PATH + "export", ((uint)_pin).ToString());

					// set i/o direction
					File.WriteAllText(GPIO_PATH + "gpio" + (uint)_pin + "/direction", value.ToString().ToLower());
				}
			}
		}

		#endregion

        #region Class Methods
        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        public override void Write(bool value)
        {
			base.Write(value);
			File.WriteAllText(GPIO_PATH + "gpio" + (uint)_pin + "/value", value ? "1" : "0");
        }

        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        public void Write(PinState value)
        {
            Write(value == PinState.High);
        }

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public override PinState Read()
        {
			base.Read();
			var readValue = File.ReadAllText(GPIO_PATH + "gpio" + (uint)_pin + "/value");
			return (readValue.Length > 0 && readValue[0] == '1') ? PinState.High : PinState.Low;
        }

        /// <summary>
        /// Dispose of the GPIO pin
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
			File.WriteAllText(GPIO_PATH + "unexport", ((uint)_pin).ToString());
        }
        #endregion
    }
}