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
	///                  
	///                  P5-03 = GPI28
	///                  P5-04 = GPI29
	///                  P5-05 = GPI30
	///                  P5-06 = GPI31
	/// 
	/// So to turn on Pin7 on the GPIO connector, pass in enumGPIOPIN.gpio4 as the pin parameter
	/// </remarks>
	public enum GPIOPins : uint
	{

		GPIO_NONE = uint.MaxValue,

		//Revision 1

		GPIO_00 = 0,
		GPIO_01 = 1,
		GPIO_04 = 4,
		GPIO_07 = 7,
		GPIO_08 = 8,
		GPIO_09 = 9,
		GPIO_10 = 10,
		GPIO_11 = 11,
		GPIO_14 = 14,
		GPIO_15 = 15,
		GPIO_17 = 17,
		GPIO_18 = 18,
		GPIO_21 = 21,
		GPIO_22 = 22,
		GPIO_23 = 23,
		GPIO_24 = 24,
		GPIO_25 = 25,

		Pin_P1_03 = 0,
		Pin_P1_05 = 1,
		Pin_P1_07 = 4,
		Pin_P1_08 = 14,
		Pin_P1_10 = 15,
		Pin_P1_11 = 17,
		Pin_P1_12 = 18,
		Pin_P1_13 = 21,
		Pin_P1_15 = 22,
		Pin_P1_16 = 23,
		Pin_P1_18 = 24,
		Pin_P1_19 = 10,
		Pin_P1_21 = 9,
		Pin_P1_22 = 25,
		Pin_P1_23 = 11,
		Pin_P1_24 = 8,
		Pin_P1_26 = 7,
		LED = 16,

		//Revision 2

		V2_GPIO_00 = 0,
		V2_GPIO_02 = 2,
		V2_GPIO_03 = 3,
		V2_GPIO_01 = 1,
		V2_GPIO_04 = 4,
		V2_GPIO_07 = 7,
		V2_GPIO_08 = 8,
		V2_GPIO_09 = 9,
		V2_GPIO_10 = 10,
		V2_GPIO_11 = 11,
		V2_GPIO_14 = 14,
		V2_GPIO_15 = 15,
		V2_GPIO_17 = 17,
		V2_GPIO_18 = 18,
		V2_GPIO_21 = 21,
		V2_GPIO_22 = 22,
		V2_GPIO_23 = 23,
		V2_GPIO_24 = 24,
		V2_GPIO_25 = 25,
		V2_GPIO_27 = 27,

		//Revision 2, new plug P5
		V2_GPIO_28 = 28,
		V2_GPIO_29 = 29,
		V2_GPIO_30 = 30,
		V2_GPIO_31 = 31,

		V2_Pin_P1_03 = 2,
		V2_Pin_P1_05 = 3,
		V2_Pin_P1_07 = 4,
		V2_Pin_P1_08 = 14,
		V2_Pin_P1_10 = 15,
		V2_Pin_P1_11 = 17,
		V2_Pin_P1_12 = 18,
		V2_Pin_P1_13 = 27,
		V2_Pin_P1_15 = 22,
		V2_Pin_P1_16 = 23,
		V2_Pin_P1_18 = 24,
		V2_Pin_P1_19 = 10,
		V2_Pin_P1_21 = 9,
		V2_Pin_P1_22 = 25,
		V2_Pin_P1_23 = 11,
		V2_Pin_P1_24 = 8,
		V2_Pin_P1_26 = 7,
		V2_LED = 16,

		//Revision 2, new plug P5
		V2_Pin_P5_03 = 28,
		V2_Pin_P5_04 = 29,
		V2_Pin_P5_05 = 30,
		V2_Pin_P5_06 = 31,

	};

	public enum GPIOPinMask : uint
	{

		GPIO_NONE = uint.MaxValue,

		//Revision 1

		GPIO_00 = 0,
		GPIO_01 = 1,
		GPIO_04 = 1 << 4,
		GPIO_07 = 1 << 7,
		GPIO_08 = 1 << 8,
		GPIO_09 = 1 << 9,
		GPIO_10 = 1 << 10,
		GPIO_11 = 1 << 11,
		GPIO_14 = 1 << 14,
		GPIO_15 = 1 << 15,
		GPIO_17 = 1 << 17,
		GPIO_18 = 1 << 18,
		GPIO_21 = 1 << 21,
		GPIO_22 = 1 << 22,
		GPIO_23 = 1 << 23,
		GPIO_24 = 1 << 24,
		GPIO_25 = 1 << 25,

		Pin_P1_03 = 1 << 0,
		Pin_P1_05 = 1 << 1,
		Pin_P1_07 = 1 << 4,
		Pin_P1_08 = 1 << 14,
		Pin_P1_10 = 1 << 15,
		Pin_P1_11 = 1 << 17,
		Pin_P1_12 = 1 << 18,
		Pin_P1_13 = 1 << 21,
		Pin_P1_15 = 1 << 22,
		Pin_P1_16 = 1 << 23,
		Pin_P1_18 = 1 << 24,
		Pin_P1_19 = 1 << 10,
		Pin_P1_21 = 1 << 9,
		Pin_P1_22 = 1 << 25,
		Pin_P1_23 = 1 << 11,
		Pin_P1_24 = 1 << 8,
		Pin_P1_26 = 1 << 7,
		LED = 1 << 16,

		//Revision 2

		V2_GPIO_00 = 1 << 0,
		V2_GPIO_02 = 1 << 2,
		V2_GPIO_03 = 1 << 3,
		V2_GPIO_01 = 1 << 1,
		V2_GPIO_04 = 1 << 4,
		V2_GPIO_07 = 1 << 7,
		V2_GPIO_08 = 1 << 8,
		V2_GPIO_09 = 1 << 9,
		V2_GPIO_10 = 1 << 10,
		V2_GPIO_11 = 1 << 11,
		V2_GPIO_14 = 1 << 14,
		V2_GPIO_15 = 1 << 15,
		V2_GPIO_17 = 1 << 17,
		V2_GPIO_18 = 1 << 18,
		V2_GPIO_21 = 1 << 21,
		V2_GPIO_22 = 1 << 22,
		V2_GPIO_23 = 1 << 23,
		V2_GPIO_24 = 1 << 24,
		V2_GPIO_25 = 1 << 25,
		V2_GPIO_27 = 1 << 27,

		//Revision 2, new plug P5
		V2_GPIO_28 = 1 << 28,
		V2_GPIO_29 = 1 << 29,
		V2_GPIO_30 = 1 << 30,
		V2_GPIO_31 = (uint)1 << 31,

		V2_Pin_P1_03 = 1 << 2,
		V2_Pin_P1_05 = 1 << 3,
		V2_Pin_P1_07 = 1 << 4,
		V2_Pin_P1_08 = 1 << 14,
		V2_Pin_P1_10 = 1 << 15,
		V2_Pin_P1_11 = 1 << 17,
		V2_Pin_P1_12 = 1 << 18,
		V2_Pin_P1_13 = 1 << 27,
		V2_Pin_P1_15 = 1 << 22,
		V2_Pin_P1_16 = 1 << 23,
		V2_Pin_P1_18 = 1 << 24,
		V2_Pin_P1_19 = 1 << 10,
		V2_Pin_P1_21 = 1 << 9,
		V2_Pin_P1_22 = 1 << 25,
		V2_Pin_P1_23 = 1 << 11,
		V2_Pin_P1_24 = 1 << 8,
		V2_Pin_P1_26 = 1 << 7,
		V2_LED = 1 << 16,

		//Revision 2, new plug P5
		V2_Pin_P5_03 = 1 << 28,
		V2_Pin_P5_04 = 1 << 29,
		V2_Pin_P5_05 = 1 << 30,
		V2_Pin_P5_06 = (uint)1 << 31,

	};

}
