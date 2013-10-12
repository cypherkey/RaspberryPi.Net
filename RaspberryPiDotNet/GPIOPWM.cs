namespace RaspberryPiDotNet
{
    public enum GPIOPwmDivisor : uint
    {
        CLK_1 = 1,          ///< 1 = 4.6875kHz, same as divider 4096
        CLK_2 = 2,          ///< 2 = 9.6MHz, fastest you can get
        CLK_4 = 4,          ///< 4 = 4.8MHz
        CLK_8 = 8,          ///< 8 = 2.4MHz
        CLK_16 = 16,        ///< 16 = 1.2MHz
        CLK_32 = 32,        ///< 32 = 600.0kHz
        CLK_64 = 64,        ///< 64 = 300kHz
        CLK_128 = 128,      ///< 128 = 150kHz
        CLK_256 = 256,      ///< 256 = 75kHz
        CLK_512 = 512,      ///< 512 = 37.5kHz
        CLK_1024 = 1024,    ///< 1024 = 18.75kHz
        CLK_2048 = 2048,    ///< 2048 = 9.375kHz
        CLK_4096 = 4096,    ///< 4096 = 4.6875kHz
        CLK_8192 = 8192,    ///< 8192 = 2.34375kHz
        CLK_16384 = 16384,  ///< 16384 = 1171.8Hz
        CLK_32768 = 32768,  ///< 32768 = 585Hz
    }
    public enum GPIOPwmChannel : uint
    {
        Channel_0 = 0,
        Channel_1 = 1,
    }
    public enum GPIOPwmMarkspace : uint
    {
        MarkSpace = 1,
        Balanced = 0,
    }
    public enum GPIOPwmEnabled : uint
    {
        False = 0,
        True = 1,
    }
}
