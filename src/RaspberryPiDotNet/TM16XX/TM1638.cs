using System;

// This class is the base class for the TM1638/TM1640 board.
// It is a port of the TM1638 library by Ricardo Batista
// URL: http://code.google.com/p/tm1638-library/
namespace RaspberryPiDotNet.TM16XX
{
    public class TM1638 : TM16XX
    {
        public TM1638(GPIO data, GPIO clock, GPIO strobe, bool activateDisplay, byte intensity)
            : base(data, clock, strobe, 8, activateDisplay, intensity)
        {
        }

        public enum TM1638_LED_COLOR : byte
        {
            RED = 1,
            GREEN = 2
        };

        public void setDisplayToHexNumber(ulong number, byte dots, bool leadingZeros)
        {
            setDisplayToString(number.ToString("x").ToUpper());
        }

        public void setDisplayToDecNumberAt(ulong number, byte dots, byte startingPos, bool leadingZeros)
        {
            if (number > 99999999L)
                setDisplayToError();
            else
            {
                for (int i = 0; i < _displays - startingPos; i++)
                {
                    if (number != 0)
                    {
                        setDisplayDigit((byte)(number % 10), (byte)(_displays - i - 1), (dots & (1 << i)) != 0);
                        number /= 10;
                    }
                    else
                    {
                        if (leadingZeros)
                        {
                            setDisplayDigit((byte)0, (byte)(_displays - i - 1), (dots & (1 << i)) != 0);
                        }
                        else
                        {
                            clearDisplayDigit((byte)(_displays - i - 1), (dots & (1 << i)) != 0);
                        }
                    }
                }
            }
        }

        public void setDisplayToDecNumber(ulong number, byte dots, bool leadingZeros)
        {
            setDisplayToDecNumberAt(number, dots, 0, leadingZeros);
        }

        public void setDisplayToSignedDecNumber(long number, byte dots, bool leadingZeros)
        {
            if (number >= 0)
            {
                setDisplayToDecNumberAt((ulong)number, dots, 0, leadingZeros);
            }
            else
            {
                if (-number > 9999999L)
                {
                    setDisplayToError();
                }
                else
                {
                    setDisplayToDecNumberAt((ulong)-number, dots, 1, leadingZeros);
                    sendChar(0, (byte)charMap['-'], (dots & (0x80)) != 0);
                }
            }
        }

        public void setDisplayToBinNumber(byte number, byte dots)
        {
            for (int i = 0; i < _displays; i++)
            {
                setDisplayDigit((byte)((number & (1 << i)) == 0 ? 0 : 1), (byte)(_displays - i - 1), (dots & (1 << i)) != 0);
            }
        }

        public void setLED(TM1638_LED_COLOR color, byte pos)
        {
            sendData((byte)((pos << 1) + 1), (byte)color);
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
            sendData((byte)(pos << 1), (byte)(data | (dot ? Convert.ToByte("10000000", 2) : Convert.ToByte("00000000", 2))));
        }
    }
}
