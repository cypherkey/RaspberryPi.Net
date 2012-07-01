using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

// Author: Aaron Anderson <aanderson@netopia.ca>
// Based on work done by x4m and britguy (http://www.raspberrypi.org/phpBB3/viewtopic.php?f=34&t=6720)
namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi GPIO using the file-based access method.
    /// </summary>
    public class GPIOFile : GPIO, IDisposable
    {
        /// <summary>
        /// The path on the Raspberry Pi for the GPIO interface
        /// </summary>
        private const string GPIO_PATH = "/sys/class/gpio/";

        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIOFile(GPIOPins pin)
            : base(pin,DirectionEnum.OUT,false)
        {
        }

        /// <summary>
        /// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="direction">Direction</param>
        public GPIOFile(GPIOPins pin, DirectionEnum direction)
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
        public GPIOFile(GPIOPins pin, DirectionEnum direction, bool initialValue)
            : base(pin, direction, initialValue)
        {
            ExportPin(pin, direction);
            Write(pin, initialValue);
        }

        ~GPIOFile()
        {
            UnexportPin(_pin);
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
            // If the pin is already exported, check it's in the proper direction
            if (_exportedPins.Keys.Contains((int)pin))
                // If the direction matches, return out of the function. If not, change the direction
                if (_exportedPins[(int)pin] == direction)
                    return;
                else
                {
                    // Set the direction on the pin and update the exported list
                    File.WriteAllText(GPIO_PATH + "gpio" + GetGPIONumber(pin) + "/direction", direction.ToString().ToLower());
                    _exportedPins[(int)pin] = direction;
                    return;
                }

            if (!Directory.Exists(GPIO_PATH + "gpio" + GetGPIONumber(pin)))
            {
                Debug.WriteLine("Exporting " + GetGPIONumber(pin));
                //export
                File.WriteAllText(GPIO_PATH + "export", GetGPIONumber(pin));
            }

            // set i/o direction
            Debug.WriteLine("Setting direction on pin " + pin + "/gpio " + (int)pin + " as " + direction);
            File.WriteAllText(GPIO_PATH + "gpio" + GetGPIONumber(pin) + "/direction", direction.ToString().ToLower());

            // Update the list of exported pins
            _exportedPins[(int)pin] = direction;
        }

        /// <summary>
        /// Unexport the GPIO. This removes the /sys/class/gpio/gpioXX directory.
        /// </summary>
        /// <param name="pin">The pin to unexport</param>
        private static void UnexportPin(GPIOPins pin)
        {
            Debug.WriteLine("unexporting pin " + pin);
            File.WriteAllText(GPIO_PATH + "unexport", GetGPIONumber(pin));

            // Remove the pin from the list of exported pins
            _exportedPins.Remove((int)pin);
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

            string writeValue = value ? "1" : "0";
            File.WriteAllText(GPIO_PATH + "gpio" + GetGPIONumber(pin) + "/value", writeValue);
            Debug.WriteLine("output to pin " + pin + "/gpio " + (int)pin + ", value was " + value);
        }

        /// <summary>
        /// Static method to read a value to the specified pin. When using the static methods ensure you use the CleanUp() method when done.
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <returns>The value read from the pin</returns>
        public static bool Read(GPIOPins pin)
        {
            bool returnValue = false;

            ExportPin(pin, DirectionEnum.IN);

            string filename = GPIO_PATH + "gpio" + GetGPIONumber(pin) + "/value";
            if (File.Exists(filename))
            {
                string readValue = File.ReadAllText(filename);
                if (readValue.Length > 0 && readValue[0] == '1')
                    returnValue = true;
            }
            else
                throw new Exception(string.Format("Cannot read from {0}. File does not exist", pin));

            Debug.WriteLine("input from pin " + pin + "/gpio " + (int)pin + ", value was " + returnValue);

            return returnValue;
        }

        public static void CleanUp()
        {
            // Loop over all exported pins and unexported them
            foreach (GPIOPins pin in _exportedPins.Keys)
            {
                UnexportPin(pin);
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
            // Call the static method
            Write(_pin, value);
        }

        /// <summary>
        /// Read a value from the pin
        /// </summary>
        /// <returns>The value read from the pin</returns>
        public override bool Read()
        {
            // Call the static method
            return Read(_pin);
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