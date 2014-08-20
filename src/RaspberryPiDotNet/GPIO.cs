using System;
using System.Collections.Generic;

// Author: Aaron Anderson <aanderson@netopia.ca>

namespace RaspberryPiDotNet
{
	/// <summary>
	/// Abstract class for the GPIO connector on the Pi (P1) (as found next to the yellow RCA video socket on the Rpi circuit board)
	/// </summary>
	public abstract class GPIO : IDisposable
	{
		/// <summary>
		/// Dictionary that stores created (exported) pins that where not disposed.
		/// </summary>
		private static Dictionary<GPIOPins, GPIO> _exportedPins = new Dictionary<GPIOPins, GPIO>();

		/// <summary>
		/// The currently assigned GPIO pin. Used for class methods.
		/// </summary>
		protected readonly GPIOPins _pin;

		/// <summary>
		/// Variable to track the disposed state
		/// </summary>
		private bool _disposed = false;

        /// <summary>
        /// Direction of the GPIO pin
        /// </summary>
		private GPIODirection _direction;

        /// <summary>
        /// GPIO pull up-down resistor
        /// </summary>
        // BCM2835_GPIO_PUD_OFF = 0b00 = 0
        // BCM2835_GPIO_PUD_DOWN = 0b01 = 1
        // BCM2835_GPIO_PUD_UP = 0b10 = 2
        public GPIOResistor _resistor = GPIOResistor.OFF;

		/// <summary>
		/// Gets the pin that this GPIO instance represents
		/// </summary>
		public GPIOPins Pin {
			get {
				if (_disposed)
					throw new ObjectDisposedException(string.Empty);
				return _pin;
			}
		}

		/// <summary>
		/// Gets the bit mask of this pin.
		/// </summary>
		public GPIOPinMask Mask {
			get {
				return (GPIOPinMask)(1 << (ushort)Pin); //Pin-Value has a low range (0-~32), so even casting to byte would be ok.
			}
		}

		/// <summary>
		/// Gets or sets the communication direction for this pin
		/// </summary>
		public virtual GPIODirection PinDirection {
			get {
				if (_disposed)
					throw new ObjectDisposedException(string.Empty);
				return _direction;
			}
			set {
				if (_disposed)
					throw new ObjectDisposedException(string.Empty);
				_direction = value;
			}
		}

        /// <summary>
        /// Gets or sets the internal resistor value for the pin
        /// </summary>
        public virtual GPIOResistor Resistor
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(string.Empty);
                return _resistor;
            }
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(string.Empty);
                _resistor = value;
            }
        }

		/// <summary>
		/// Gets the disposal state of this GPIO instance
		/// </summary>
		public bool IsDisposed {
			get {
				return _disposed;
			}
		}

		/// <summary>
		/// Access to the specified GPIO setup with the specified direction with the specified initial value
		/// </summary>
		/// <param name="pin">The GPIO pin</param>
		/// <param name="direction">Direction</param>
		/// <param name="initialValue">Initial Value</param>
		protected GPIO(GPIOPins pin, GPIODirection direction, bool initialValue) {
			if (pin == GPIOPins.GPIO_NONE) throw new ArgumentException("Invalid pin");
			lock (_exportedPins) {
				if (_exportedPins.ContainsKey(pin))
					throw new Exception("Cannot use pin with multiple instances. Unexport the previous instance with Dispose() first! (pin " + (uint)pin + ")");
				_exportedPins[pin] = this;

				_pin = pin;
				try {
					PinDirection = direction;
					Write(initialValue);
				}
				catch {
					Dispose();
					throw;
				}
			}
		}

		/// <summary>
		/// Finalizer to make sure we cleanup after ourselves.
		/// </summary>
		~GPIO() {
			if (!_disposed)
				Dispose();
		}

		/// <summary>
		/// Sets a pin to output the give value.
		/// 
		/// Creates (exports) the pin if needed, and sets it to Out direction.
		/// </summary>
		/// <param name="pin">The pin who's value to set</param>
		/// <param name="value">The value to set</param>
		public static void Write(GPIOPins pin, bool value) {
			CreatePin(pin, GPIODirection.Out).Write(value);
		}

		/// <summary>
		/// Gets the value of a given pin.
		/// 
		/// Creates (exports) the pin if needed, and sets it to In direction.
		/// </summary>
		/// <param name="pin">The pin who's value to get</param>
		/// <returns>The value of the pin</returns>
		public static PinState Read(GPIOPins pin) {
			return CreatePin(pin, GPIODirection.In).Read();
		}

		/// <summary>
		/// Creates a pin if it has not already been created (exported), creates a GPIOMem if possible, otherwise falls back to GPIOFile.
		/// </summary>
		/// <param name="pin">The pin to create or export</param>
		/// <param name="dir">The direction the pin is to have</param>
		/// <returns>The GPIO instance representing the pin</returns>
		public static GPIO CreatePin(GPIOPins pin, GPIODirection dir) {
			lock (_exportedPins)
				if (_exportedPins.ContainsKey(pin)) {
					if (_exportedPins[pin].PinDirection != dir)
						_exportedPins[pin].PinDirection = dir;
					return _exportedPins[pin];
				}

			try {
				return new GPIOMem(pin, dir);
			}
#if DEBUG
			catch (Exception e) {
				System.Diagnostics.Debug.WriteLine("Unable to create pin " + (uint)pin + " as GPIOMem because: " + e.ToString());
			}
#else
			catch //stuff like lib load problems, wrong exports, etc...
			{
			}
#endif
			try {
				return new GPIOFile(pin, dir);
			}
#if DEBUG
			catch (Exception e) {
				System.Diagnostics.Debug.WriteLine("Unable to create pin " + (uint)pin + " as GPIOFile because: " + e.ToString());
			}
#else
			catch //stuff like GPIO Sys FS does not exist or is not responding, open by another process, etc...
			{
			}
#endif

#if DEBUG
			System.Diagnostics.Debug.WriteLine("Using debug GPIO pin for pin number " + (uint)pin);
			return new GPIODebug(pin, dir);
#else
			throw new Exception("Cannot access GPIO pin " + (uint)pin + ". Make sure libbcm2835.so is accessible, or that GPIO SYSFS is working and not in use by another process");
#endif
		}

		/// <summary>
		/// Write a value to the pin
		/// </summary>
		/// <param name="value">The value to write to the pin</param>
		public virtual void Write(bool value) {
			if (IsDisposed)
				throw new ObjectDisposedException(string.Empty);
			if (_direction != GPIODirection.Out)
				PinDirection = GPIODirection.Out;
		}

		/// <summary>
		/// Read a value from the pin
		/// </summary>
		/// <returns>The value read from the pin</returns>
		public virtual PinState Read() {
			if (IsDisposed)
				throw new ObjectDisposedException(string.Empty);
			return PinState.Low;
		}

		/// <summary>
		/// Dispose of the GPIO pin
		/// </summary>
		public virtual void Dispose() {
			if (_disposed)
				throw new ObjectDisposedException(string.Empty);

			_disposed = true;
			lock (_exportedPins) {
				_exportedPins.Remove(_pin);
			}
		}
	}
}
