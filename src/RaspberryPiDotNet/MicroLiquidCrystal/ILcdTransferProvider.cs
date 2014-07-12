// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0

namespace RaspberryPiDotNet.MicroLiquidCrystal
{
    public interface ILcdTransferProvider
    {
        void Send(byte data, bool mode, bool backlight);

        /// <summary>
        /// Specify if the provider works in 4-bit mode; 8-bit mode is used otherwise.
        /// </summary>
        bool FourBitMode { get; }
    }
}
