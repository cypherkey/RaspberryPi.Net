using System;

// Code modified from original - GPIOLcdTransferProvider.cs
// Original code uses license:
// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0
namespace RaspberryPiDotNet.MicroLiquidCrystal
{
    /// <summary>
    /// Raspberry Pi GPIO provider for the Micro Liquid Crystal Library.
    /// </summary>
    public class RaspPiGPIOMemLcdTransferProvider : IDisposable, ILcdTransferProvider
    {
        private readonly GPIOMem _rsPort;
        private readonly GPIOMem _rwPort;
        private readonly GPIOMem _enablePort;
        private readonly GPIOMem[] _dataPorts;
        private readonly bool _fourBitMode;
        private bool _disposed;

        public RaspPiGPIOMemLcdTransferProvider(GPIOPins rs, GPIOPins enable, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7)
            : this(true, rs, GPIOPins.GPIO_NONE, enable, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, d4, d5, d6, d7)
        { }

        public RaspPiGPIOMemLcdTransferProvider(GPIOPins rs, GPIOPins rw, GPIOPins enable, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7)
            : this(true, rs, rw, enable, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, GPIOPins.GPIO_NONE, d4, d5, d6, d7)
        { }

        public RaspPiGPIOMemLcdTransferProvider(GPIOPins rs, GPIOPins enable, GPIOPins d0, GPIOPins d1, GPIOPins d2, GPIOPins d3, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7)
            : this(false, rs, GPIOPins.GPIO_NONE, enable, d0, d1, d2, d3, d4, d5, d6, d7)
        { }

        public RaspPiGPIOMemLcdTransferProvider(GPIOPins rs, GPIOPins rw, GPIOPins enable, GPIOPins d0, GPIOPins d1, GPIOPins d2, GPIOPins d3, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7)
            : this(false, rs, rw, enable, d0, d1, d2, d3, d4, d5, d6, d7)
        { }

        /// <summary>
        /// Creates a variable of type LiquidCrystal. The display can be controlled using 4 or 8 data lines. If the former, omit the pin numbers for d0 to d3 and leave those lines unconnected. The RW pin can be tied to ground instead of connected to a pin on the Arduino; if so, omit it from this function's parameters. 
        /// </summary>
        /// <param name="fourBitMode"></param>
        /// <param name="rs">The number of the CPU pin that is connected to the RS (register select) pin on the LCD.</param>
        /// <param name="rw">The number of the CPU pin that is connected to the RW (Read/Write) pin on the LCD (optional).</param>
        /// <param name="enable">the number of the CPU pin that is connected to the enable pin on the LCD.</param>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="d4"></param>
        /// <param name="d5"></param>
        /// <param name="d6"></param>
        /// <param name="d7"></param>
        public RaspPiGPIOMemLcdTransferProvider(bool fourBitMode, GPIOPins rs, GPIOPins rw, GPIOPins enable, 
                                                 GPIOPins d0, GPIOPins d1, GPIOPins d2, GPIOPins d3, 
                                                 GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7)
        {
            _fourBitMode = fourBitMode;

            if (rs == GPIOPins.GPIO_NONE) throw new ArgumentException("rs");
            _rsPort = new GPIOMem(rs);

            // we can save 1 pin by not using RW. Indicate by passing GPIO.GPIOPins.GPIO_NONE instead of pin#
            if (rw != GPIOPins.GPIO_NONE) // (RW is optional)
                _rwPort = new GPIOMem(rw);

            if (enable == GPIOPins.GPIO_NONE) throw new ArgumentException("enable");
            _enablePort = new GPIOMem(enable);

            var dataPins = new[] { d0, d1, d2, d3, d4, d5, d6, d7};
            _dataPorts = new GPIOMem[8];
            for (int i = 0; i < 8; i++)
            {
                if (dataPins[i] != GPIOPins.GPIO_NONE)
                    _dataPorts[i] = new GPIOMem(dataPins[i]);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~RaspPiGPIOMemLcdTransferProvider()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _rsPort.Dispose();
                _rwPort.Dispose();
                _enablePort.Dispose();

                for (int i = 0; i < 8; i++)
                {
                    if (_dataPorts[i] != null)
                        _dataPorts[i].Dispose();
                }
                _disposed = true;
            }
            
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public bool FourBitMode
        {
            get { return _fourBitMode; }
        }

        /// <summary>
        /// Write either command or data, with automatic 4/8-bit selection
        /// </summary>
        /// <param name="value">value to write</param>
        /// <param name="mode">Mode for RS (register select) pin.</param>
        /// <param name="backlight">Backlight state.</param>
        public void Send(byte value, bool mode, bool backlight)
        {
            if (_disposed)
                throw new ObjectDisposedException("NetopiaGPIOLcdTransferProvider");

            //TODO: set backlight

            _rsPort.Write(mode);

            // if there is a RW pin indicated, set it low to Write
            if (_rwPort != null)
            {
                _rwPort.Write(false);
            }

            if (!_fourBitMode)
            {
                Write8Bits(value);
            }
            else
            {
                Write4Bits((byte) (value >> 4));
                Write4Bits(value);
            }
        }

        private void Write8Bits(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                _dataPorts[i].Write(((value >> i) & 0x01) == 0x01);
            }

            PulseEnable();
        }

        private void Write4Bits(byte value)
        {
            for (int i = 0; i < 4; i++)
            {
                _dataPorts[4+i].Write(((value >> i) & 0x01) == 0x01);
            }

            PulseEnable();

            // The bcm2835 library is so fast that a delay is required.
            System.Threading.Thread.Sleep(1);
        }

        private void PulseEnable()
        {
            _enablePort.Write(false);
            _enablePort.Write(true);  // enable pulse must be >450ns
            _enablePort.Write(false); // commands need > 37us to settle
        }
    }
}
