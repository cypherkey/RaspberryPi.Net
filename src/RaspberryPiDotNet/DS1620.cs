using System;
using System.Threading;

// Derived based on work done by AdamS at http://forums.netduino.com/index.php?/topic/3335-netduino-plus-and-ds1620-anyone/page__view__findpost__p__22972

namespace RaspberryPiDotNet
{
    public class DS1620
    {
        private GPIO _dq;
        private GPIO _clk;
        private GPIO _rst;

        public DS1620(GPIO dq, GPIO clk, GPIO rst)
        {
            _dq = dq;
            _clk = clk;
            _rst = rst;
        }

        /// <summary>
        /// The current temperature
        /// </summary>
        public double Temperature
        {
            get
            {
                return GetTemperature();
            }
        }

        /// <summary>
        /// Sends 8 bit command to the DS1620
        /// </summary>
        /// <param name="command">The command</param>
        private void SendCommand(int command)
        {
            // Sends 8 bit command on DQ output, least sig bit first
            int n;
            for (n = 0; n < 8; n++) 
            {            
                var bit = ((command >> n) & (0x01));
                _dq.Write((bit == 1));
                _clk.Write(false);
                _clk.Write(true);      
            }   
        }

        /// <summary>
        /// Read 8 bit data from the DS1620
        /// </summary>
        /// <returns>The temperature in half degree increments</returns>
        private int ReadData()
        {
            int n;
            var raw_data = 0;            // go into input mode         

            for (n = 0; n < 9; n++)
            {
                _clk.Write(false);
                var bit = _dq.Read() == PinState.High ? 1 : 0;
                _clk.Write(true);
                raw_data = raw_data | (bit << n);
            }
            Console.WriteLine("bin=" + Convert.ToString(raw_data,2));
            return (raw_data);
        }

        /// <summary>
        /// Send the commands to retrieve the temperature
        /// </summary>
        /// <returns>The temperature with a half degree granularity</returns>
        private double GetTemperature()
        {
            _rst.Write(false);
            _clk.Write(true);
            _rst.Write(true);
            SendCommand(0x0c); // write config command        
            SendCommand(0x02); // cpu mode       
            _rst.Write(false);
            Thread.Sleep(200); // wait until the configuration register is written        
            _clk.Write(true);
            _rst.Write(true);
            SendCommand(0xEE); // start conversion     
            _rst.Write(false);
            Thread.Sleep(200);
            _clk.Write(true);
            _rst.Write(true);
            SendCommand(0xAA);
            int raw_data = ReadData();
            _rst.Write(false);
            return ((double)raw_data / 2.0);   
        }
    }
}
