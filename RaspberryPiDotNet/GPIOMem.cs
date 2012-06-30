using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Based on the code by x4m at http://www.raspberrypi.org/phpBB3/viewtopic.php?f=34&t=6720&start=25
// Downloaded from https://dl.dropbox.com/u/7610280/GpioProgram.cs
namespace RaspberryPiDotNet
{
    public class GPIOMem : GPIO, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Access to the specified GPIO setup as an output port with an initial value of false (0)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        public GPIOMem(GPIOPins pin)
            : base(pin,DirectionEnum.OUT,false)
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
    }
}
