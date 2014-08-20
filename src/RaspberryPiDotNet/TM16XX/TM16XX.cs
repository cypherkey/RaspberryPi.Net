using System;
using System.Collections.Generic;

// This class is the base class for the TM1638/TM1640 board.
// It is a port of the TM1638 library by Ricardo Batista
// URL: http://code.google.com/p/tm1638-library/

namespace RaspberryPiDotNet.TM16XX
{
    public abstract class TM16XX
    {

        // ReSharper disable InconsistentNaming
        protected GPIO _data;
        protected GPIO _clock;
        protected GPIO _strobe;
        protected int _displays;
        // ReSharper restore InconsistentNaming
        /// <summary>
        /// The character map for the seven segment displays.
        /// The bits are displayed by mapping bellow
        ///  -- 0 --
        /// |       |
        /// 5       1
        ///  -- 6 --
        /// 4       2
        /// |       |
        ///  -- 3 --  .7
        /// </summary>
        public readonly static Dictionary<char, byte> charMap = new Dictionary<char, byte>()
        {
            { ' ', Convert.ToByte("00000000",2) },
            { '!', Convert.ToByte("10000110",2) },
            { '"', Convert.ToByte("00100010",2) },
            { '#', Convert.ToByte("01111110",2) },
            { '$', Convert.ToByte("01101101",2) },
            { '%', Convert.ToByte("00000000",2) },
            { '&', Convert.ToByte("00000000",2) },
            { '\'', Convert.ToByte("00000010",2) },
            { '(', Convert.ToByte("00110000",2) },
            { ')', Convert.ToByte("00000110",2) },
            { '*', Convert.ToByte("01100011",2) },
            { '+', Convert.ToByte("00000000",2) },
            { ',', Convert.ToByte("00000100",2) },
            { '-', Convert.ToByte("01000000",2) },
            { '.', Convert.ToByte("10000000",2) },
            { '/', Convert.ToByte("01010010",2) },
            { '0', Convert.ToByte("00111111",2) },
            { '1', Convert.ToByte("00000110",2) },
            { '2', Convert.ToByte("01011011",2) },
            { '3', Convert.ToByte("01001111",2) },
            { '4', Convert.ToByte("01100110",2) },
            { '5', Convert.ToByte("01101101",2) },
            { '6', Convert.ToByte("01111101",2) },
            { '7', Convert.ToByte("00100111",2) },
            { '8', Convert.ToByte("01111111",2) },
            { '9', Convert.ToByte("01101111",2) },
            { ':', Convert.ToByte("00000000",2) },
            { ';', Convert.ToByte("00000000",2) },
            { '<', Convert.ToByte("00000000",2) },
            { '=', Convert.ToByte("01001000",2) },
            { '>', Convert.ToByte("00000000",2) },
            { '?', Convert.ToByte("01010011",2) },
            { '@', Convert.ToByte("01011111",2) },
            { 'A', Convert.ToByte("01110111",2) },
            { 'B', Convert.ToByte("01111111",2) },
            { 'C', Convert.ToByte("00111001",2) },
            { 'D', Convert.ToByte("00111111",2) },
            { 'E', Convert.ToByte("01111001",2) },
            { 'F', Convert.ToByte("01110001",2) },
            { 'G', Convert.ToByte("00111101",2) },
            { 'H', Convert.ToByte("01110110",2) },
            { 'I', Convert.ToByte("00000110",2) },
            { 'J', Convert.ToByte("00011111",2) },
            { 'K', Convert.ToByte("01101001",2) },
            { 'L', Convert.ToByte("00111000",2) },
            { 'M', Convert.ToByte("00010101",2) },
            { 'N', Convert.ToByte("00110111",2) },
            { 'O', Convert.ToByte("00111111",2) },
            { 'P', Convert.ToByte("01110011",2) },
            { 'Q', Convert.ToByte("01100111",2) },
            { 'R', Convert.ToByte("00110001",2) },
            { 'S', Convert.ToByte("01101101",2) },
            { 'T', Convert.ToByte("01111000",2) },
            { 'U', Convert.ToByte("00111110",2) },
            { 'V', Convert.ToByte("00101010",2) },
            { 'W', Convert.ToByte("00011101",2) },
            { 'X', Convert.ToByte("01110110",2) },
            { 'Y', Convert.ToByte("01101110",2) },
            { 'Z', Convert.ToByte("01011011",2) },
            { '[', Convert.ToByte("00111001",2) },
            { '\\', Convert.ToByte("01100100",2) },
            { ']', Convert.ToByte("00001111",2) },
            { '^', Convert.ToByte("00000000",2) },
            { '_', Convert.ToByte("00001000",2) },
            { '`', Convert.ToByte("00100000",2) },
            { 'a', Convert.ToByte("01011111",2) },
            { 'b', Convert.ToByte("01111100",2) },
            { 'c', Convert.ToByte("01011000",2) },
            { 'd', Convert.ToByte("01011110",2) },
            { 'e', Convert.ToByte("01111011",2) },
            { 'f', Convert.ToByte("00110001",2) },
            { 'g', Convert.ToByte("01101111",2) },
            { 'h', Convert.ToByte("01110100",2) },
            { 'i', Convert.ToByte("00000100",2) },
            { 'j', Convert.ToByte("00001110",2) },
            { 'k', Convert.ToByte("01110101",2) },
            { 'l', Convert.ToByte("00110000",2) },
            { 'm', Convert.ToByte("01010101",2) },
            { 'n', Convert.ToByte("01010100",2) },
            { 'o', Convert.ToByte("01011100",2) },
            { 'p', Convert.ToByte("01110011",2) },
            { 'q', Convert.ToByte("01100111",2) },
            { 'r', Convert.ToByte("01010000",2) },
            { 's', Convert.ToByte("01101101",2) },
            { 't', Convert.ToByte("01111000",2) },
            { 'u', Convert.ToByte("00011100",2) },
            { 'v', Convert.ToByte("00101010",2) },
            { 'w', Convert.ToByte("00011101",2) },
            { 'x', Convert.ToByte("01110110",2) },
            { 'y', Convert.ToByte("01101110",2) },
            { 'z', Convert.ToByte("01000111",2) },
            { '{', Convert.ToByte("01000110",2) },
            { '|', Convert.ToByte("00000110",2) },
            { '}', Convert.ToByte("01110000",2) },
            { '~', Convert.ToByte("00000001",2) }
        };

        public TM16XX(GPIO data, GPIO clock, GPIO strobe, int displays, bool activateDisplay, int intensity)
        {
            // Set the pins
            _data = data;
            _clock = clock;
            _strobe = strobe;
            _displays = displays;

            _strobe.Write(true);
            _clock.Write(true);

            sendCommand(0x40);
            sendCommand((byte)(0x80 | (activateDisplay ? 0x08 : 0x00) | Math.Min(7, intensity)));

            _strobe.Write(false);
            send(0xC0);
            for (int i = 0; i < 16; i++)
            {
                send(0x00);
            }
            _strobe.Write(true);
        }

        public bool ActivateDisplay
        {
            set
            {
                if (value)
                    sendCommand(0x88);
                else
                    sendCommand(0x80);
            }
        }
        
        public void setupDisplay(bool active, int intensity)
        {
            sendCommand((byte)(0x80 | (active ? 8 : 0) | Math.Min(7, intensity)));

            // necessary for the TM1640
            _strobe.Write(false);
            _clock.Write(false);
            _clock.Write(true);
            _strobe.Write(true);
        }

        public void setDisplayDigit(byte digit, byte pos, bool dot)
        {
            char chr = Char.Parse(digit.ToString());
            if (charMap.ContainsKey(chr))
                sendChar(pos, charMap[digit.ToString()[0]], dot);
        }

        public void setDisplayToError()
        {
            byte[] error = new byte[5] {
                charMap['E'],
                charMap['r'],
                charMap['r'],
                charMap['o'],
                charMap['r']
            };
            setDisplay(error, 5);

	        for (int i = 8; i < _displays; i++)
            {
	            clearDisplayDigit((byte)i, false);
	        }
        }

        public void clearDisplayDigit(byte pos, bool dot)
        {
            sendChar(pos, 0, dot);
        }

        public void setDisplay(byte[] values, int size)
        {
            for (int i = 0; i < size; i++)
            {
                sendChar((byte)i, values[i], false);
            }
        }

        public void clearDisplay()
        {
            for (int i = 0; i < _displays; i++)
            {
                sendData((byte)(i << 1), (byte)0);
            }
        }

        public void setDisplayToString(string str)
        {
            setDisplayToString(str, 0, 0);
        }

        public void setDisplayToString(string str, byte dots, byte pos)
        {
            int stringLength = str.Length;

            for (int i = 0; i < _displays - pos; i++)
                if (i < stringLength)
                    sendChar((byte)(i + (int)pos), (byte)(charMap[str[i]]), (dots & (1 << (_displays - i - 1))) != 0);
                else
                    break;
        }

        public void sendCommand(byte cmd)
        {
            _strobe.Write(false);
            send(cmd);
            _strobe.Write(true);
        }

        public void sendData(byte address, byte data)
        {
            sendCommand(0x44);
            _strobe.Write(false);
            send((byte)(0xC0 | address));
            send(data);
            _strobe.Write(true);
        }

        public void send(byte data)
        {
            for (int i = 0; i < 8; i++)
            {
                _clock.Write(false);
                _data.Write((data & 1) > 0 ? true : false);
                data >>= 1;
                _clock.Write(true);
            }
        }

        public byte receive()
        {
            byte temp = 0;

            // Pull-up on
            _data.Write(true);

            for (int i = 0; i < 8; i++)
            {
                temp >>= 1;
                _clock.Write(false);

                if (_data.Read() == PinState.High)
                    temp |= 0x80;

                _clock.Write(true);
            }

            _data.Write(false);

            return temp;
        }

        public abstract void sendChar(byte pos, byte data, bool dot);
    }
}
