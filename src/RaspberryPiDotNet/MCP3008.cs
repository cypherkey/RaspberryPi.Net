using System;

// Original author: Mikey Sklar - git://gist.github.com/3249416.git
// Adafruit article: http://learn.adafruit.com/reading-a-analog-in-and-controlling-audio-volume-with-the-raspberry-pi
// Ported from python and modified by: Gilberto Garcia <ferraripr@gmail.com>; twitter: @ferraripr

namespace RaspberryPiDotNet
{
    /// <summary>
    /// Raspberry Pi using MCP3008 A/D Converters with SPI Serial Interface
    /// <seealso cref="http://ww1.microchip.com/downloads/en/DeviceDoc/21295d.pdf"/>
    /// </summary>
    public class MCP3008
    {
        private int adcnum;
        private GPIO clockpin;
        private GPIO mosipin;
        private GPIO misopin;
        private GPIO cspin;

        /// <summary>
        /// Connect MCP3008 with clock, Serial Peripheral Interface(SPI) and channel
        /// </summary>
        /// <param name="adc_channel">MCP3008 channel number 0-7 (pin 1-8 on chip).</param>
        /// <param name="SPICLK">Clock pin</param>
        /// <param name="SPIMOSI">SPI Master Output, Slave Input (MOSI)</param>
        /// <param name="SPIMISO">SPI Master Input, Slave Output (MISO)</param>
        /// <param name="SPICS">SPI Chip Select</param>
        public MCP3008(int adc_channel, GPIO SPICLK, GPIO SPIMOSI, GPIO SPIMISO, GPIO SPICS)
        {
            adcnum = adc_channel;
            clockpin = SPICLK;
            mosipin = SPIMOSI;
            misopin = SPIMISO;
            cspin = SPICS;
            //
            if (adc_channel >= 0 && adc_channel <= 7)
            {
                //This is the range we are looking for, from CH0 to CH7. 
            }
            else
            {
                throw new IndexOutOfRangeException("MCP3008 Channel Input is out of range, Channel input should be from 0-7 (8-Channel).");
            }
        }

        /// <summary>
        /// Analog to digital conversion
        /// </summary>
        public int AnalogToDigital
        {
            get
            {
                return readadc();
            }
        }

        private int readadc()
        {
            if ((adcnum > 7) || adcnum < 0)
            {
                return -1;
            }
            cspin.Write(true);

            clockpin.Write(false); // #start clock low
            cspin.Write(false); // #bring CS low

            int commandout = adcnum;
            commandout |= 0x18; //# start bit + single-ended bit
            commandout <<= 3;  //# we only need to send 5 bits here

            for (int i = 0; i < 5; i++)
            {
                if ((commandout & 0x80) == 128)
                {
                    mosipin.Write(true);
                }
                else
                {
                    mosipin.Write(false);
                }
                commandout <<= 1;
                clockpin.Write(true);
                clockpin.Write(false);
            }

            int adcout = 0;
            //# read in one empty bit, one null bit and 10 ADC bits
            for (var i = 0; i < 12; i++)
            {
                clockpin.Write(true);
                clockpin.Write(false);
                adcout <<= 1;
                if (misopin.Read() == PinState.High)
                    adcout |= 0x1;
            }

            cspin.Write(true);
            adcout /= 2;//# first bit is 'null' so drop it
            return adcout;
        }

    }
}
