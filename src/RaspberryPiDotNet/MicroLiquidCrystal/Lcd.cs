using System;
using System.Text;
using System.Threading;

// Micro Liquid Crystal Library
// http://microliquidcrystal.codeplex.com
// Appache License Version 2.0 

namespace RaspberryPiDotNet.MicroLiquidCrystal
{
    public class Lcd
    {
        private static readonly byte[] RowOffsets = new byte[] { 0x00, 0x40, 0x14, 0x54 };

        private readonly ILcdTransferProvider _provider;
        private bool _showCursor;
        private bool _blinkCursor;
        private bool _visible = true;
        private bool _autoScroll;
        private bool _backlight = true;

        private byte _numLines;
        private byte _numColumns;
        private byte _currLine;
        private byte _displayFunction;

        #region LCD Flags
        // ReSharper disable InconsistentNaming

        // commands
        private const byte LCD_CLEARDISPLAY = 0x01;
        private const byte LCD_RETURNHOME = 0x02;
        private const byte LCD_ENTRYMODESET = 0x04;
        private const byte LCD_DISPLAYCONTROL = 0x08;
        private const byte LCD_CURSORSHIFT = 0x10;
        private const byte LCD_FUNCTIONSET = 0x20;
        private const byte LCD_SETCGRAMADDR = 0x40;
        private const byte LCD_SETDDRAMADDR = 0x80;

        // flags for display entry mode
        private const byte LCD_ENTRYRIGHT = 0x00;
        private const byte LCD_ENTRYLEFT = 0x02;
        private const byte LCD_ENTRYSHIFTINCREMENT = 0x01;
        private const byte LCD_ENTRYSHIFTDECREMENT = 0x00;

        // flags for display on/off control
        private const byte LCD_DISPLAYON = 0x04;
        private const byte LCD_DISPLAYOFF = 0x00;
        private const byte LCD_CURSORON = 0x02;
        private const byte LCD_CURSOROFF = 0x00;
        private const byte LCD_BLINKON = 0x01;
        private const byte LCD_BLINKOFF = 0x00;

        // flags for display/cursor shift
        private const byte LCD_DISPLAYMOVE = 0x08;
        private const byte LCD_CURSORMOVE = 0x00;
        private const byte LCD_MOVERIGHT = 0x04;
        private const byte LCD_MOVELEFT = 0x00;

        // flags for function set
        private const byte LCD_4BITMODE  = 0x00;
        private const byte LCD_8BITMODE  = 0x10;
        private const byte LCD_1LINE     = 0x00;
        private const byte LCD_2LINE     = 0x08;
        private const byte LCD_5x8DOTS   = 0x00;
        private const byte LCD_5x10DOTS  = 0x04;

        // ReSharper restore InconsistentNaming
        #endregion

        public Lcd(ILcdTransferProvider provider)
        {
            Encoding = Encoding.UTF8;

            if (provider == null) throw new ArgumentNullException("provider");
            _provider = provider;

            if (_provider.FourBitMode)
                _displayFunction = LCD_4BITMODE | LCD_1LINE | LCD_5x8DOTS;
            else
                _displayFunction = LCD_8BITMODE | LCD_1LINE | LCD_5x8DOTS;

            Begin(16, 1);
        }

        protected ILcdTransferProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Display or hide the LCD cursor: an underscore (line) at the position to which the next character will be written. 
        /// </summary>
        public bool ShowCursor
        {
            get { return _showCursor; }
            set {
                if (_showCursor != value)
                {
                    _showCursor = value;
                    UpdateDisplayControl();
                }
            }
        }

        /// <summary>
        /// Display or hide the blinking block cursor at the position to which the next character will be written.
        /// </summary>
        public bool BlinkCursor
        {
            get { return _blinkCursor; }
            set
            {
                if (_blinkCursor != value)
                {
                    _blinkCursor = value;
                    UpdateDisplayControl();
                }
            }
        }

        /// <summary>
        /// Turns the LCD display on or off. This will restore the text (and cursor) that was on the display. 
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    UpdateDisplayControl();
                }
            }
        }


        /// <summary>
        /// Turns the LCD backlight on or off.
        /// </summary>
        public bool Backlight
        {
            get { return _backlight; }
            set
            {
                if (_backlight != value)
                {
                    _backlight = value;
                    UpdateDisplayControl();
                }
            }
        }

/*       
        /// <summary>
        /// Turns on automatic scrolling of the LCD. This causes each character output to the display to push previous characters 
        /// over by one space. If the current text direction is left-to-right (the default), the display scrolls to the left; 
        /// if the current direction is right-to-left, the display scrolls to the right. 
        /// This has the effect of outputting each new character to the same location on the LCD. 
        /// </summary>
        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                //TODO:
            }
        }*/

        /// <summary>
        /// Get or set the encoding used to map the string into bytes codes that are sent LCD. 
        /// UTF8 is used by default.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Use this method to initialize the LCD. Specifies the dimensions (width and height) of the display. 
        /// </summary>
        /// <param name="columns">The number of columns that the display has.</param>
        /// <param name="lines">The number of rows that the display has.</param>
        public void Begin(byte columns, byte lines)
        {
            Begin(columns, lines, true, false);
        }

        public void Begin(byte columns, byte lines, bool leftToRight, bool dotSize)
        {
            if (lines > 1)
            {
                _displayFunction |= LCD_2LINE;
            }
            _currLine = 0;
            _numLines = lines;
            _numColumns = columns;

            // for some 1 line displays you can select a 10 pixel high font
            if (dotSize && (lines == 1))
            {
                _displayFunction |= LCD_5x10DOTS;
            }

            Thread.Sleep(50);      // LCD controller needs some warm-up time
            // rs, rw, and enable should be low by default

            if (Provider.FourBitMode)
            {
                // this is according to the hitachi HD44780 datasheet
                // figure 24, pg 46

                // we start in 8bit mode, try to set 4 bit mode
                SendCommand(0x03);
                Thread.Sleep(5);   // wait min 4.1ms

                SendCommand(0x03);
                Thread.Sleep(5);    // wait min 4.1ms

                // third go!
                SendCommand(0x03);
                Thread.Sleep(5);

                // finally, set to 4-bit interface
                SendCommand(0x02);
            }
            else
            {
                // this is according to the hitachi HD44780 datasheet
                // page 45 figure 23

                // Send function set command sequence
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
                Thread.Sleep(5); // wait more than 4.1ms

                // second try
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
                Thread.Sleep(1);

                // third go
                SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));
            }

            // finally, set # lines, font size, etc.
            SendCommand((byte) (LCD_FUNCTIONSET | _displayFunction));

            // turn the display on with no cursor or blinking default
            _visible = true;
            _showCursor = false;
            _blinkCursor = false;
            _backlight = true;
            UpdateDisplayControl();

            // clear it off
            Clear();

            // set the entry mode
            var displayMode = leftToRight ? LCD_ENTRYLEFT : LCD_ENTRYRIGHT;
            displayMode |= LCD_ENTRYSHIFTDECREMENT;
            SendCommand((byte)(LCD_ENTRYMODESET | displayMode));
        }

        /// <summary>
        /// Clears the LCD screen and positions the cursor in the upper-left corner.
        /// </summary>
        public void Clear()
        {
            SendCommand(LCD_CLEARDISPLAY);
            Thread.Sleep(2); // this command takes a long time!
        }

        /// <summary>
        /// Positions the cursor in the upper-left of the LCD. 
        /// That is, use that location in outputting subsequent text to the display. 
        /// To also clear the display, use the <see cref="Clear"/> method instead. 
        /// </summary>
        public void Home()
        {
            SendCommand(LCD_RETURNHOME);
            Thread.Sleep(2); // this command takes a long time!
        }

        /// <summary>
        /// Position the LCD cursor; that is, set the location at which subsequent text written to the LCD will be displayed
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        public void SetCursorPosition(int column, int row)
        {
            if (row > _numLines)
                row = _numLines - 1;
            
            int address = column + RowOffsets[row];
            SendCommand((byte) (LCD_SETDDRAMADDR | address));
        }

        /// <summary>
        /// Scrolls the contents of the display (text and cursor) one space to the left. 
        /// </summary>
        public void ScrollDisplayLeft()
        {
            //TODO: test
            SendCommand(0x18 | 0x00);
        }

        /// <summary>
        /// Scrolls the contents of the display (text and cursor) one space to the right. 
        /// </summary>
        public void ScrollDisplayRight()
        {
            //TODO: test
            SendCommand(0x18 | 0x04);
        }

        /// <summary>
        /// Moves cursor left or right.
        /// </summary>
        /// <param name="right">true to move cursor right.</param>
        public void MoveCursor(bool right)
        {
            //TODO: verify this instruction
            SendCommand((byte)(0x10 | ((right) ? 0x04 : 0x00)));
        }

        /// <summary>
        /// Writes a text to the LCD.
        /// </summary>
        /// <param name="text">The string to write.</param>
        public void Write(string text)
        {
            byte[] buffer = Encoding.GetBytes(text);
            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a specified number of bytes to the LCD using data from a buffer.
        /// </summary>
        /// <param name="buffer">The byte array that contains data to write to display.</param>
        /// <param name="offset">The zero-based byte offset in the buffer parameter at which to begin copying bytes to display.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            int len = offset + count;
            for (int i = offset; i < len; i++)
            {
                WriteByte(buffer[i]);
            }
        }

        /// <summary>
        /// Sends one data byte to the display.
        /// </summary>
        /// <param name="data">The data byte to send.</param>
        public void WriteByte(byte data)
        {
            Provider.Send(data, true, _backlight);
        }

        /// <summary>
        /// Sends HD44780 lcd interface command.
        /// </summary>
        /// <param name="data">The byte command to send.</param>
        public void SendCommand(byte data)
        {
            Provider.Send(data, false, _backlight);
        }

        /// <summary>
        /// Create a custom character (gylph) for use on the LCD.
        /// </summary>
        /// <remarks>
        /// Up to eight characters of 5x8 pixels are supported (numbered 0 to 7). 
        /// The appearance of each custom character is specified by an array of eight bytes, one for each row.
        /// The five least significant bits of each byte determine the pixels in that row. 
        /// To display a custom character on the screen, call WriteByte() and pass its number. 
        /// </remarks>
        /// <param name="location">Which character to create (0 to 7) </param>
        /// <param name="charmap">The character's pixel data </param>
        /// <param name="offset">Offset in the charmap wher character data is found </param>
        public void CreateChar(int location, byte[] charmap, int offset)
        {
            location &= 0x7; // we only have 8 locations 0-7
            SendCommand((byte) (LCD_SETCGRAMADDR | (location << 3)));
            for (int i = 0; i < 8; i++)
            {
                WriteByte(charmap[offset+i]);
            }
        }

        public void CreateChar(int location, byte[] charmap)
        {
            CreateChar(location, charmap, 0);
        }

        /// <summary>
        /// Method is called when any of the display control properties are changed.
        /// </summary>
        protected void UpdateDisplayControl()
        {
            int command = LCD_DISPLAYCONTROL;
            command |= (_visible) ? LCD_DISPLAYON : LCD_DISPLAYOFF;
            command |= (_showCursor) ? LCD_CURSORON : LCD_CURSOROFF;
            command |= (_blinkCursor) ? LCD_BLINKON : LCD_BLINKOFF;
            
            //NOTE: backlight is updated with each command
            SendCommand((byte)command); 
        }
    }
}
