using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaspberryPiDotNet
{
    public class TM1638 : TM16XX
    {
        private byte TM1638_COLOR_RED = 1;
        private byte TM1638_COLOR_GREEN = 2;

        public TM1638(GPIO data, GPIO clock, GPIO strobe, bool activateDisplay, byte intensity)
            : base(data, clock, strobe, 8, activateDisplay, intensity)
        {
        }

        public void setDisplayToHexNumber(ulong number, byte dots, bool leadingZeros, byte[] numberFont)
        {
            for (int i = 0; i < _displays; i++)
            {
                if (!leadingZeros && number == 0)
                    clearDisplayDigit((byte)(_displays - i - 1), (dots & (1 << i)) != 0);
                else
                {
                    setDisplayDigit((byte)(number & 0xF), (byte)(_displays - i - 1), (dots & (1 << i)) != 0, numberFont);
                    number >>= 4;
                }
            }
        }

        public void setDisplayToDecNumberAt(ulong number, byte dots, byte startingPos, bool leadingZeros, byte[] numberFont)
        {
            if (number > 99999999L)
                setDisplayToError();
            else
            {
                for (int i = 0; i < _displays - startingPos; i++)
                {
                    if (number != 0)
                    {
                        setDisplayDigit((byte)(number % 10), (byte)(_displays - i - 1), (dots & (1 << i)) != 0, numberFont);
                        number /= 10;
                    }
                    else
                    {
                        if (leadingZeros)
                        {
                            setDisplayDigit((byte)0, (byte)(_displays - i - 1), (dots & (1 << i)) != 0, numberFont);
                        }
                        else
                        {
                            clearDisplayDigit((byte)(_displays - i - 1), (dots & (1 << i)) != 0);
                        }
                    }
                }
            }
        }

        public void setDisplayToDecNumber(ulong number, byte dots, bool leadingZeros, byte[] numberFont)
        {
            setDisplayToDecNumberAt(number, dots, 0, leadingZeros, numberFont);
        }

        public void setDisplayToSignedDecNumber(long number, byte dots, bool leadingZeros, byte[] numberFont)
        {
            if (number >= 0)
            {
                setDisplayToDecNumberAt((ulong)number, dots, 0, leadingZeros, numberFont);
            }
            else
            {
                if (-number > 9999999L)
                {
                    setDisplayToError();
                }
                else
                {
                    setDisplayToDecNumberAt((ulong)-number, dots, 1, leadingZeros, numberFont);
                    sendChar(0, (byte)charMap['-'], (dots & (0x80)) != 0);
                }
            }
        }

        public void setDisplayToBinNumber(byte number, byte dots, byte[] numberFont)
        {
            for (int i = 0; i < _displays; i++)
            {
                setDisplayDigit((byte)((number & (1 << i)) == 0 ? 0 : 1), (byte)(_displays - i - 1), (dots & (1 << i)) != 0, numberFont);
            }
        }

        public void setLED(byte color, byte pos)
        {
            sendData((byte)((pos << 1) + 1), color);
        }

        void setLEDs(ushort leds)
        {
            for (int i = 0; i < _displays; i++)
            {
                byte color = 0;

                if ((leds & (1 << i)) != 0)
                {
                    color |= TM1638_COLOR_RED;
                }

                if ((leds & (1 << (i + 8))) != 0)
                {
                    color |= TM1638_COLOR_GREEN;
                }

                setLED(color, (byte)i);
            }
        }

        public byte getButtons()
        {
            byte keys = 0;

            _strobe.Write(false);
            send(0x42);
            for (byte i = 0; i < 4; i++)
            {
                keys |= (byte)(receive() << i);
            }
            _strobe.Write(true);

            return keys;
        }

        public override void sendChar(byte pos, byte data, bool dot)
        {
            sendData((byte)(pos << 1), (byte)(data | (dot ? Convert.ToByte("10000000", 2) : 0)));
        }
    }
}
