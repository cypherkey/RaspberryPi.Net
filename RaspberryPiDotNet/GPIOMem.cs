using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi GPIO using the direct memory access method. This requires the bcm2835 GPIO library provided by 
    /// Mike McCauley (mikem@open.com.au) at http://www.open.com.au/mikem/bcm2835/index.html.
    /// 
    /// To create the shared object, download the source code from the link above. The standard Makefile compiled a
    /// statically linked library. In the src directory run:
    ///    make libbcm2835.a
    ///    cc -shared bcm2835.o -o libbcm2835.so
    /// </summary>
    public class GPIOMem : GPIO, IDisposable
    {

        private static bool _initialized = false;

        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIOMem(GPIOPins pin)
            : base(pin,DirectionEnum.OUT,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIOMem(GPIOPins pin, DirectionEnum direction)
            : base(pin, direction, false)
        {
            ExportPin(pin, direction);
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with the specified initial value
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        /// <param name="initialValue">Initial Value</param>
        public GPIOMem(GPIOPins pin, DirectionEnum direction, bool initialValue)
            : base(pin, direction, initialValue)
        {
            ExportPin(pin, direction);
            Write(pin, initialValue);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Initialize the memory access to the GPIO
        /// </summary>
        /// <returns></returns>
        private static bool Initialize()
        {
            bool ret = true;
            if (!_initialized)
            {
                // initialize the mapped memory
                ret = bcm2835_init();
                _initialized = true;
            }

            return ret;
        }

        /// <summary>
        /// Export the GPIO setting the direction. This creates the /sys/class/gpio/gpioXX directory.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction"></param>
        private static void ExportPin(GPIOPins pin, DirectionEnum direction)
        {
            Initialize();

            // If the pin is already exported, check it's in the proper direction
            if (_exportedPins.Keys.Contains((int)pin))
                // If the direction matches, return out of the function. If not, change the direction
                if (_exportedPins[(int)pin] == direction)
                    return;
                else
                {
                    // Set the direction on the pin and update the exported list
                    bcm2835_gpio_fsel((uint)pin, direction == DirectionEnum.IN ? (uint)0 : (uint)1);
                    _exportedPins[(int)pin] = direction;
                    return;
                }
        }

        /// <summary>
        /// Static method to write a value to the specified pin. When using the static methods ensure you use the CleanUp() method when done.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="value">The value to write to the pin</param>
        public static void Write(GPIOPins pin, bool value)
        {
            if (pin == GPIOPins.GPIO_NONE)
                return;

            ExportPin(pin, DirectionEnum.OUT);

            bcm2835_gpio_write((uint)pin, value ? (uint)1 : (uint)0);
            Debug.WriteLine("output to pin " + pin + "/gpio " + (int)pin + ", value was " + value);
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Write a value to the pin
        /// </summary>
        /// <param name="value">The value to write to the pin</param>
        public override void Write(bool value)
        {
            Write(_pin, value);
        }

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public override bool Read()
        {
            return false;
        }

        /// <summary>
        /// Dispose of the GPIO pin
        /// </summary>
        public override void Dispose()
        {
        }
        #endregion

        #region Imported functions
        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_init")]
        static extern bool bcm2835_init();

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_fsel")]
        static extern void bcm2835_gpio_fsel(uint pin, uint mode);

        [DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write")]
        static extern void bcm2835_gpio_write(uint pin, uint value);
        #endregion
    }
}
