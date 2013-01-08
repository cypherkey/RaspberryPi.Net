namespace RaspberryPiDotNet
{
    /// <remarks>
    /// Refer to http://elinux.org/Rpi_Low-level_peripherals for diagram.
    /// P1-01 = bottom left, P1-02 = top left
    /// pi connector P1 pin    = GPIOnum
    ///                  P1-03 = GPIO0
    ///                  P1-05 = GPIO1
    ///                  P1-07 = GPIO4
    ///                  P1-08 = GPIO14 - alt function (UART0_TXD) on boot-up
    ///                  P1-10 = GPIO15 - alt function (UART0_TXD) on boot-up
    ///                  P1-11 = GPIO17
    ///                  P1-12 = GPIO18
    ///                  P1-13 = GPIO21
    ///                  P1-15 = GPIO22
    ///                  P1-16 = GPIO23
    ///                  P1-18 = GPIO24
    ///                  P1-19 = GPIO10
    ///                  P1-21 = GPIO9
    ///                  P1-22 = GPIO25
    ///                  P1-23 = GPIO11
    ///                  P1-24 = GPIO8
    ///                  P1-26 = GPIO7
    /// So to turn on Pin7 on the GPIO connector, pass in enumGPIOPIN.gpio4 as the pin parameter
    /// </remarks>
	public enum GPIOPins : uint
	{
		GPIO_NONE = uint.MaxValue,
		GPIO00 = 0,
		GPIO02_REV2 = 2,
		GPIO03_REV2 = 3,
		GPIO01 = 1,
		GPIO04 = 4,
		GPIO07 = 7,
		GPIO08 = 8,
		GPIO09 = 9,
		GPIO10 = 10,
		GPIO11 = 11,
		GPIO14 = 14,
		GPIO15 = 15,
		GPIO17 = 17,
		GPIO18 = 18,
		GPIO21 = 21,
		GPIO22 = 22,
		GPIO23 = 23,
		GPIO24 = 24,
		GPIO25 = 25,
		GPIO27_REV2 = 27,
		Pin03 = 0,
		Pin03_REV2 = 2,
		Pin05 = 1,
		Pin05_REV2 = 3,
		Pin07 = 4,
		Pin08 = 14,
		Pin10 = 15,
		Pin11 = 17,
		Pin12 = 18,
		Pin13 = 21,
		Pin13_REV2 = 27,
		Pin15 = 22,
		Pin16 = 23,
		Pin18 = 24,
		Pin19 = 10,
		Pin21 = 9,
		Pin22 = 25,
		Pin23 = 11,
		Pin24 = 8,
		Pin26 = 7,
		LED = 16,
	};
}
