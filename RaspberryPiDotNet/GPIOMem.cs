using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RaspberryPiDotNet
{
	/// <summary>
	/// Raspberry Pi GPIO using the direct memory access method.
	/// This requires the bcm2835 GPIO library provided by 
	/// Mike McCauley (mikem@open.com.au) at http://www.open.com.au/mikem/bcm2835/index.html.
	/// 
	/// To create the shared object, download the source code from the link above. The standard Makefile compiles a
	/// statically linked library. To build a shared object, do:
	///    tar -zxf bcm2835-1.3.tar.gz
	///    cd bcm2835-1.3/src
	///    make libbcm2835.a
	///    cc -shared bcm2835.o -o libbcm2835.so
	/// Place the shared object in the same directory as the executable and other assemblies.
	/// </summary>
	public class GPIOMem : GPIO
	{
		#region Static Constructor
		static GPIOMem() {
			// initialize the mapped memory
			if (!bcm2835_init())
				throw new Exception("Unable to initialize bcm2835.so library");
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Access to the specified GPIO setup as an output port with an initial value of false (0)
		/// </summary>
		/// <param name="pin">The GPIO pin</param>
		public GPIOMem(GPIOPins pin)
			: this(pin, GPIODirection.Out, false) {
		}

		/// <summary>
		/// Access to the specified GPIO setup with the specified direction with an initial value of false (0)
		/// </summary>
		/// <param name="pin">The GPIO pin</param>
		/// <param name="direction">Direction</param>
		public GPIOMem(GPIOPins pin, GPIODirection direction)
			: this(pin, direction, false) {
		}

		/// <summary>
		/// Access to the specified GPIO setup with the specified direction with the specified initial value
		/// </summary>
		/// <param name="pin">The GPIO pin</param>
		/// <param name="direction">Direction</param>
		/// <param name="initialValue">Initial Value</param>
		public GPIOMem(GPIOPins pin, GPIODirection direction, bool initialValue)
			: base(pin, direction, initialValue) {
		}

        /// <summary>
        /// Access to the specified GPIO PWM setup with the specified pin, divisor, channel, markspace, range and status of PWM (Enabled/disabled)
        /// </summary>
        /// <param name="pin">The GPIO pin</param>
        /// <param name="PWM_Divisor">Clock Divider from 19.2MHz</param>
        /// <param name="PWM_Channel">Selected Pwm Channel (use channel 0)</param>
        /// <param name="PWM_Markspace">Selected Pwm method (balanced or markspace, use markspace default)</param>
        /// <param name="PWM_Range">Selected range for Pwm steps (Output frequency is calculated from Pwm_Divisor/Pwm_range)</param>
        /// <param name="PWM_Enabled">Selected intialised Pwm mode(Enabled or disabled)</param>
        public GPIOMem(GPIOPins pin, GPIOPwmDivisor PWM_Divisor, GPIOPwmChannel PWM_Channel, GPIOPwmMarkspace PWM_Markspace, int PWM_Range, GPIOPwmEnabled PWM_Enabled)
            : base(pin, PWM_Divisor, PWM_Channel, PWM_Markspace, PWM_Range, PWM_Enabled)
        {
            bcm2835_gpio_fsel(pin, (int)GPIODirection.Pwm);
            bcm2835_pwm_set_clock((int)PWM_Divisor);
            bcm2835_pwm_set_mode((int)PWM_Channel, (int)PWM_Markspace, (int)PWM_Enabled);
            bcm2835_pwm_set_range((int)PWM_Channel, PWM_Range);
        }

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the communication direction for this pin
		/// </summary>
		public override GPIODirection PinDirection {
			get {
				return base.PinDirection;
			}
			set {
				if (PinDirection != (base.PinDirection = value)) // Left to right eval ensures base class gets to check for disposed object access
				{
					// Set the direction on the pin
					bcm2835_gpio_fsel(_pin, value == GPIODirection.Out);
                    if (value == GPIODirection.In)
                        Resistor = GPIOResistor.OFF;
				}
			}
		}

        /// <summary>
        /// Gets or sets the internal resistor value for the pin
        /// </summary>
        public override GPIOResistor Resistor
        {
            get {
                return base.Resistor;
            }
            set {
                if (Resistor != (base.Resistor = value)) // Left to right eval ensures base class gets to check for disposed object access
                {
                    bcm2835_gpio_set_pud(_pin, (uint)value);
                }
            }
        }
		#endregion

		#region Class Methods
		/// <summary>
		/// Write a value to the pin
		/// </summary>
		/// <param name="value">The value to write to the pin</param>
		public override void Write(bool value) {
			base.Write(value);
			bcm2835_gpio_write(_pin, value);
		}

		/// <summary>
		/// Read a value from the pin
		/// </summary>
		/// <returns>The value read from the pin</returns>
		public override bool Read() {
			base.Read();
			return bcm2835_gpio_lev(_pin);
		}

        public override void Set_PWM_Clock(int PWM_Divisor)
        {
            bcm2835_pwm_set_clock(PWM_Divisor);
             
        }
        public override void Set_PWM_Mode(int markspace, int enabled)
        {
            bcm2835_pwm_set_mode((int)base.Channel, markspace, enabled);
        }
        public override void Set_PWM_Range(int range)
        {
            bcm2835_pwm_set_range((int)base.Channel, range);
        }

        public override void Set_PWM_Data(int range)
        {
            bcm2835_pwm_set_data((int)base.Channel, range);
        }

		#endregion

		#region Imported functions
		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_init")]
		static extern bool bcm2835_init();

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_fsel")]
		static extern void bcm2835_gpio_fsel(GPIOPins pin, int mode_out);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write")]
		static extern void bcm2835_gpio_write(GPIOPins pin, bool value);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_lev")]
		static extern bool bcm2835_gpio_lev(GPIOPins pin);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_set_pud")]
		static extern void bcm2835_gpio_set_pud(GPIOPins pin, int pud);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_set_multi")]
		static extern void bcm2835_gpio_set_multi(GPIOPinMask mask);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_clr_multi")]
		static extern void bcm2835_gpio_clr_multi(GPIOPinMask mask);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_gpio_write_multi")]
		static extern void bcm2835_gpio_write_multi(GPIOPinMask mask, bool on);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_clock")]
		static extern void bcm2835_pwm_set_clock(int divisor);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_mode")]
		static extern void bcm2835_pwm_set_mode(int channel, int markspace, int enabled);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_range")]
		static extern void bcm2835_pwm_set_range(int channel, int range);

		[DllImport("libbcm2835.so", EntryPoint = "bcm2835_pwm_set_data")]
		static extern void bcm2835_pwm_set_data(int channel, int data);

		#endregion

		/// <summary>
		/// Sets any of the first 32 GPIO output pins specified in the mask to HIGH.
		/// </summary>
		/// <param name="mask">Mask of pins to affect. Use eg: (GPIOPinMask.GPIO_00) | GPIOPinMask.GPIO_01)</param>
		public static void SetMulti(GPIOPinMask mask) {
			bcm2835_gpio_set_multi(mask);
		}

		/// <summary>
		/// Sets any of the first 32 GPIO output pins specified in the mask to LOW.
		/// </summary>
		/// <param name="mask">Mask of pins to affect. Use eg: (GPIOPinMask.GPIO_00) | GPIOPinMask.GPIO_01)</param>
		public static void ClearMulti(GPIOPinMask mask) {
			bcm2835_gpio_clr_multi(mask);
		}

		/// <summary>
		/// Sets any of the first 32 GPIO output pins specified in the mask to value.
		/// </summary>
		/// <param name="mask">Mask of pins to affect. Use eg: (GPIOPinMask.GPIO_00) | GPIOPinMask.GPIO_01)</param>
		public static void WriteMulti(GPIOPinMask mask, bool value) {
			bcm2835_gpio_write_multi(mask, value);
		}

	}
}
